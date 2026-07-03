using BookStoreApp.Data;
using BookStoreApp.DTOs;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(BookStoreContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Books)
                .AsNoTracking()
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    BookCount = c.Books.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET /api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    BookCount = c.Books.Count
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { Message = "Category not found!" });

            return Ok(category);
        }

        // POST /api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDTO dto)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower());

            if (exists)
                return BadRequest(new { Message = "Category already exists!" });

            var category = new Category { CategoryName = dto.CategoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category '{Name}' created", category.CategoryName);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new { category.Id, category.CategoryName });
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { Message = "Category not found!" });

            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category '{Name}' updated", category.CategoryName);
            return Ok(new { Message = "Category updated successfully!" });
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound(new { Message = "Category not found!" });

            if (category.Books.Any())
                return BadRequest(new { Message = "Cannot delete category with books!" });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category '{Name}' deleted", category.CategoryName);
            return Ok(new { Message = "Category deleted successfully!" });
        }
    }
}