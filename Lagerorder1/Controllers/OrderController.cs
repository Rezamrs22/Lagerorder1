using Microsoft.AspNetCore.Mvc;
using Lagerorder1.Data;
using Lagerorder1.Shared.Models;
using Lagerorder1.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Lagerorder1.Services;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;
using SkiaSharp;


[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .ToListAsync();

        var dtos = orders.Select(MappingService.MapToOrderDto).ToList();
        return Ok(dtos);
    }


    [HttpGet("{id:int}/barcode.svg")]
    [AllowAnonymous]
    [Produces("image/svg+xml")]
    public async Task<IActionResult> GetOrderBarcodeSvg(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order is null)
            return NotFound($"Order {id} hittades inte.");

        var writer = new BarcodeWriterSvg
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = 200,
                Height = 22,
                Margin = 1,
                PureBarcode = true
            }
        };

        var svg = writer.Write(order.OrderNumber).Content;
        Response.Headers["Content-Disposition"] = $"inline; filename=Order_{order.OrderId}.svg";
        return Content(svg, "image/svg+xml");
    }

    

    // GET /api/Order/{id}/barcode.png
    [HttpGet("{id:int}/barcode.png")]
    public async Task<IActionResult> GetOrderBarcode(int id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order is null)
            return NotFound($"Order {id} hittades inte.");

        
        var barcodePng = GenerateBarcodePng(order.OrderNumber, width: 200, height: 22, margin: 1);

        
        Response.Headers["Content-Disposition"] = $"inline; filename=Order_{order.OrderNumber}_barcode.png";
        return File(barcodePng, "image/png");
    }

    
    private static byte[] GenerateBarcodePng(string value, int width, int height, int margin)
    {
        var writer = new ZXing.SkiaSharp.BarcodeWriter
        {
            Format = ZXing.BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = margin,
                PureBarcode = true
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(value);
        using var image  = SKImage.FromBitmap(bitmap);
        using var data   = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }


    [HttpPost]
    [Consumes("application/json")]
    public async Task<ActionResult<OrderConfirmationDto>> Create([FromBody] CreateOrderDto dto)
    {
        if (dto?.OrderDetails == null || dto.OrderDetails.Count == 0)
            return BadRequest("Ordern är tom.");

        await using var tx = await _context.Database.BeginTransactionAsync();

       
        var order = new Order
        {
            CustomerName = dto.CustomerName,
            OrderDate = DateTime.UtcNow
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(); 

        // 2) Ordernummer
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{order.OrderId:D6}";
        await _context.SaveChangesAsync();

        // 3) Rader + lager
        foreach (var d in dto.OrderDetails)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == d.ProductId);
            if (product is null)
                return NotFound($"Produkt {d.ProductId} finns inte.");

            if (d.Quantity <= 0)
                return BadRequest($"Ogiltig kvantitet för produktId {d.ProductId}.");

            if (product.StockStatus < d.Quantity)
                return BadRequest($"Otillräckligt lager för {product.Name}.");

            product.StockStatus -= d.Quantity;
            product.NumberSold += d.Quantity;

            _context.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = product.Price // lås pris vid ordertillfället
            });
        }

        await _context.SaveChangesAsync();

        // 4) Streckkod (Code-128) som SVG
        var writer = new BarcodeWriterSvg
        {
            Format = BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 22,   // längre (höjd på staplarna)
                Width  = 220,  // bredare (horisontellt)
                Margin = 1,    // liten “quiet zone” runt koden
                PureBarcode = true
            }
        };
        var svg = writer.Write(order.OrderNumber).Content;

        await tx.CommitAsync();

        return Ok(new OrderConfirmationDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            BarcodeSvg = svg
        });
    }

    // PUT /api/Orders/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateOrderDto dto)
    {
        var order = await _context.Orders.Include(o => o.OrderDetails)
                                         .FirstOrDefaultAsync(o => o.OrderId == id);
        if (order is null) return NotFound();

        order.CustomerName = dto.CustomerName;
        order.OrderDetails = dto.OrderDetails.Select(od => new OrderDetail
        {
            ProductId = od.ProductId,
            Quantity = od.Quantity,
            UnitPrice = 0
        }).ToList();

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/Orders/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
