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
    public class BooksController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookStoreContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] BookFilterDTO filter)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
                query = query.Where(b => b.Title.ToLower().Contains(filter.Keyword.ToLower()));

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(b => b.Category!.CategoryName.ToLower() == filter.Category.ToLower());

            if (!string.IsNullOrEmpty(filter.Author))
                query = query.Where(b => b.Author!.AuthorName.ToLower().Contains(filter.Author.ToLower()));

            if (filter.MinPrice.HasValue)
                query = query.Where(b => b.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(b => b.Price <= filter.MaxPrice.Value);

            var total = await query.CountAsync();

            var books = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(b => new BookResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    BookType = b.BookType,
                    AuthorName = b.Author!.AuthorName,
                    CategoryName = b.Category!.CategoryName
                })
                .ToListAsync();

            return Ok(new { Total = total, Page = filter.Page, PageSize = filter.PageSize, Books = books });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    BookType = b.BookType,
                    AuthorName = b.Author!.AuthorName,
                    CategoryName = b.Category!.CategoryName
                })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound(new { Message = "Book not found!" });

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook(CreateBookDTO dto)
        {
            var author = await _context.Authors.FindAsync(dto.AuthorId);
            if (author == null)
                return BadRequest(new { Message = "Author not found!" });

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest(new { Message = "Category not found!" });

            Book book;

            if (dto.BookType == "Paperback")
            {
                book = new PaperbackBook
                {
                    Title = dto.Title,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    AuthorId = dto.AuthorId,
                    CategoryId = dto.CategoryId,
                    Pages = dto.Pages ?? 0,
                    BookType = "Paperback"
                };
            }
            else if (dto.BookType == "EBook")
            {
                book = new EBook
                {
                    Title = dto.Title,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    AuthorId = dto.AuthorId,
                    CategoryId = dto.CategoryId,
                    FileFormat = dto.FileFormat ?? "PDF",
                    BookType = "EBook"
                };
            }
            else if (dto.BookType == "AudioBook")
            {
                book = new AudioBook
                {
                    Title = dto.Title,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    AuthorId = dto.AuthorId,
                    CategoryId = dto.CategoryId,
                    Narrator = dto.Narrator ?? "",
                    Duration = dto.Duration ?? 0,
                    BookType = "AudioBook"
                };
            }
            else
            {
                return BadRequest(new { Message = "Invalid book type!" });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book added successfully");
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new { book.Id, book.Title });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDTO dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(new { Message = "Book not found!" });

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.Price.HasValue) book.Price = dto.Price.Value;
            if (dto.Stock.HasValue) book.Stock = dto.Stock.Value;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Book updated successfully");
            return Ok(new { Message = "Book updated successfully!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound(new { Message = "Book not found!" });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book deleted successfully");
            return Ok(new { Message = "Book deleted successfully!" });
        }
    }
}