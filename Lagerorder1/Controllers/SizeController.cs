using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lagerorder1.Data;
using Lagerorder1.Services;
using Lagerorder1.Shared.DTOs;
using Swashbuckle.AspNetCore.Annotations; 

namespace Lagerorder1.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/Size
    public class SizeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SizeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/Size
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SizeDto>>> GetSizes()
        {
            var dtos = await _context.Sizes
                .AsNoTracking()
                .Include(c => c.Products)
                
                .ToListAsync();

            var sizeDtos = dtos.Select(s => MappingService.MapToSizeDto(s)).ToList();

            return Ok(sizeDtos);
        }

        // GET /api/Size/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SizeDto>> GetSize(int id)
        {
            var size = await _context.Sizes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SizeId == id);

            if (size is null) return NotFound();

            var dto = MappingService.MapToSizeDto(size);
            return Ok(dto);
        }
    }
}