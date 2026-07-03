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
    public class AuthorsController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(BookStoreContext context, ILogger<AuthorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET /api/authors
        [HttpGet]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .Select(a => new AuthorResponseDTO
                {
                    Id = a.Id,
                    AuthorName = a.AuthorName,
                    Country = a.Country,
                    BookCount = a.Books.Count
                })
                .ToListAsync();

            return Ok(authors);
        }

        // GET /api/authors/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AuthorResponseDTO
                {
                    Id = a.Id,
                    AuthorName = a.AuthorName,
                    Country = a.Country,
                    BookCount = a.Books.Count
                })
                .FirstOrDefaultAsync();

            if (author == null)
                return NotFound(new { Message = "Author not found!" });

            return Ok(author);
        }

        // POST /api/authors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAuthor(CreateAuthorDTO dto)
        {
            var author = new Author
            {
                AuthorName = dto.AuthorName,
                Country = dto.Country
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Author '{Name}' created", author.AuthorName);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, new { author.Id, author.AuthorName });
        }

        // PUT /api/authors/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDTO dto)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound(new { Message = "Author not found!" });

            author.AuthorName = dto.AuthorName;
            author.Country = dto.Country;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Author '{Name}' updated", author.AuthorName);
            return Ok(new { Message = "Author updated successfully!" });
        }

        // DELETE /api/authors/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound(new { Message = "Author not found!" });

            if (author.Books.Any())
                return BadRequest(new { Message = "Cannot delete author with books!" });

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Author '{Name}' deleted", author.AuthorName);
            return Ok(new { Message = "Author deleted successfully!" });
        }
    }
}