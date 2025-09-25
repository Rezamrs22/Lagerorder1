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
    [Route("api/[controller]")] // /api/OrderDetail
    public class OrderDetailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/OrderDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetOrderDetails(CancellationToken ct)
        {
            var dtos = await _context.OrderDetails
                .AsNoTracking()
                .ToOrderDetailDtos()
                .ToListAsync(ct);

            return Ok(dtos);
        }

        // GET /api/OrderDetail/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(int id, CancellationToken ct)
        {
            var dto = await _context.OrderDetails
                .AsNoTracking()
                .Where(od => od.OrderDetailId == id)
                .ToOrderDetailDtos()
                .FirstOrDefaultAsync(ct);

            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}