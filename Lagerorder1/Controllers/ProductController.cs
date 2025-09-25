using Microsoft.AspNetCore.Mvc;
using Lagerorder1.Data;
using Lagerorder1.Shared.Models;
using Lagerorder1.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Lagerorder1.Services;
using Swashbuckle.AspNetCore.Annotations; 

namespace Lagerorder1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var dtos = await _context.Products
                .AsNoTracking()
                .MapToProductDtos()           
                .ToListAsync();

                return Ok(dtos);
        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        [SwaggerResponse(201, "Product created", typeof(ProductDto))]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var dto = MappingService.MapToProductDtos(_context.Products.Where(p => p.ProductId == product.ProductId)).FirstOrDefault();
            if (dto == null) return NotFound();
            

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}