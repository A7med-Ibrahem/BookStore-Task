using BookStoreApp.Data;
using BookStoreApp.DTOs;
using BookStoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStoreApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(BookStoreContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST /api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == User.FindFirstValue(ClaimTypes.Email));

            if (customer == null)
                return BadRequest(new { Message = "Customer not found! Please register as a customer first." });

            var purchase = new Purchase
            {
                CustomerId = customer.Id,
                PurchaseDate = DateTime.Now
            };

            foreach (var item in dto.Items)
            {
                var book = await _context.Books.FindAsync(item.BookId);
                if (book == null)
                    return BadRequest(new { Message = $"Book ID {item.BookId} not found!" });

                if (book.Stock < item.Quantity)
                    return BadRequest(new { Message = $"Not enough stock for '{book.Title}'! Available: {book.Stock}" });

                book.Stock -= item.Quantity;
                purchase.Items.Add(new PurchaseItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    PriceAtTime = book.Price
                });
            }

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Order #{Id} created for customer {Email}", purchase.Id, User.FindFirstValue(ClaimTypes.Email));

            return CreatedAtAction(nameof(GetOrder), new { id = purchase.Id }, new { purchase.Id, Message = "Order created successfully!" });
        }

        // GET /api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var isAdmin = User.IsInRole("Admin");
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var query = _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Book)
                .AsNoTracking()
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(p => p.Customer!.Email == userEmail);

            var orders = await query
                .Select(p => new OrderResponseDTO
                {
                    Id = p.Id,
                    CustomerName = p.Customer!.FullName,
                    PurchaseDate = p.PurchaseDate,
                    Total = p.Items.Sum(i => i.Quantity * i.PriceAtTime),
                    Items = p.Items.Select(i => new OrderItemResponseDTO
                    {
                        BookTitle = i.Book!.Title,
                        Quantity = i.Quantity,
                        PriceAtTime = i.PriceAtTime,
                        SubTotal = i.Quantity * i.PriceAtTime
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var isAdmin = User.IsInRole("Admin");
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var order = await _context.Purchases
                .Include(p => p.Customer)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Book)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new OrderResponseDTO
                {
                    Id = p.Id,
                    CustomerName = p.Customer!.FullName,
                    PurchaseDate = p.PurchaseDate,
                    Total = p.Items.Sum(i => i.Quantity * i.PriceAtTime),
                    Items = p.Items.Select(i => new OrderItemResponseDTO
                    {
                        BookTitle = i.Book!.Title,
                        Quantity = i.Quantity,
                        PriceAtTime = i.PriceAtTime,
                        SubTotal = i.Quantity * i.PriceAtTime
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound(new { Message = "Order not found!" });

            if (!isAdmin && order.CustomerName != _context.Customers
                .AsNoTracking()
                .FirstOrDefault(c => c.Email == userEmail)?.FullName)
                return Forbid();

            return Ok(order);
        }
    }
}