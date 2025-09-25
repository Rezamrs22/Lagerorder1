using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lagerorder1.Data;
using Lagerorder1.Services;
using Lagerorder1.Shared.DTOs;
using Swashbuckle.AspNetCore.Annotations; 

namespace Lagerorder1.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // /api/Category
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken ct)
        {
            var dtos = await _context.Categories
                .AsNoTracking()
                // Ingen Include behövs när vi projicerar
                .ToCategoryDtos()
                .ToListAsync(ct);

            return Ok(dtos);
        }

        // GET /api/Category/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken ct)
        {
            var dto = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .ToCategoryDtos()
                .FirstOrDefaultAsync(ct);

            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
