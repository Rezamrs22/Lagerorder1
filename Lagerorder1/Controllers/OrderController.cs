using Microsoft.AspNetCore.Mvc;
using Lagerorder1.Data;
using Lagerorder1.Shared.Models;
using Lagerorder1.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Lagerorder1.Services;
using Swashbuckle.AspNetCore.Annotations;


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

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order is null) return NotFound();

        var dto = MappingService.MapToOrderDto(order);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
    {
        var order = new Order
        {
            CustomerName = dto.CustomerName,
            OrderDate = DateTime.UtcNow,
            OrderDetails = dto.OrderDetails.Select(od => new OrderDetail
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                UnitPrice = 0 // sätts ev. från produkt senare
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var result = MappingService.MapToOrderDto(order);
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, CreateOrderDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
