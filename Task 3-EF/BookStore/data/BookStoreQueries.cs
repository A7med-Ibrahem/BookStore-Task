using BookStoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Data
{
    public class BookStoreQueries
    {
        private readonly BookStoreContext _context;

        public BookStoreQueries(BookStoreContext context)
        {
            _context = context;
        }

        // Task 4 - List all books with category and author
        public void ListBooksWithDetails()
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsNoTracking()
                .ToList();

            Console.WriteLine("\n📚 All Books:");
            foreach (var b in books)
                Console.WriteLine($"[{b.GetBookType()}] {b.Title} | Author: {b.Author?.AuthorName} | Category: {b.Category?.CategoryName} | Price: {b.Price:C}");
        }

        // Task 5 - Top 5 best-selling books
        public void TopBestSellingBooks()
        {
            var books = _context.PurchaseItems
                .Include(i => i.Book)
                .AsNoTracking()
                .GroupBy(i => i.Book!.Title)
                .Select(g => new { Title = g.Key, TotalSold = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            Console.WriteLine("\n🏆 Top 5 Best-Selling Books:");
            foreach (var b in books)
                Console.WriteLine($"{b.Title} — Sold: {b.TotalSold}");
        }

        // Task 6 - Customers with number of purchases
        public void CustomersWithPurchaseCount()
        {
            var customers = _context.Customers
                .Include(c => c.Purchases)
                .AsNoTracking()
                .Select(c => new { c.FullName, PurchaseCount = c.Purchases.Count })
                .OrderByDescending(c => c.PurchaseCount)
                .ToList();

            Console.WriteLine("\n👤 Customers & Purchases:");
            foreach (var c in customers)
                Console.WriteLine($"{c.FullName} — Purchases: {c.PurchaseCount}");
        }

        // Task 7 - Categories with more than 5 books
        public void CategoriesWithMoreThan5Books()
        {
            var categories = _context.Categories
                .Include(c => c.Books)
                .AsNoTracking()
                .Where(c => c.Books.Count > 5)
                .Select(c => new { c.CategoryName, BookCount = c.Books.Count })
                .ToList();

            Console.WriteLine("\n📂 Categories with more than 5 books:");
            if (!categories.Any())
                Console.WriteLine("No categories found!");
            foreach (var c in categories)
                Console.WriteLine($"{c.CategoryName} — Books: {c.BookCount}");
        }

        // Task 8 - Books above average price
        public void BooksAboveAveragePrice()
        {
            var avg = _context.Books.Average(b => b.Price);
            var books = _context.Books
                .Include(b => b.Author)
                .AsNoTracking()
                .Where(b => b.Price > avg)
                .OrderByDescending(b => b.Price)
                .ToList();

            Console.WriteLine($"\n💰 Books above average price ({avg:C}):");
            foreach (var b in books)
                Console.WriteLine($"{b.Title} — {b.Price:C}");
        }

        // Task 9 - Customers with no purchases
        public void CustomersWithNoPurchases()
        {
            var customers = _context.Customers
                .AsNoTracking()
                .Where(c => !c.Purchases.Any())
                .ToList();

            Console.WriteLine("\n🚫 Customers with no purchases:");
            if (!customers.Any())
                Console.WriteLine("All customers have purchases!");
            foreach (var c in customers)
                Console.WriteLine($"{c.FullName} — {c.Email}");
        }

        // Task 10 - Total revenue by month
        public void RevenueByMonth()
        {
            var revenue = _context.Purchases
                .Include(p => p.Items)
                .AsNoTracking()
                .GroupBy(p => new { p.PurchaseDate.Year, p.PurchaseDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Total = g.SelectMany(p => p.Items)
                             .Sum(i => i.Quantity * i.PriceAtTime)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            Console.WriteLine("\n📅 Revenue by Month:");
            foreach (var r in revenue)
                Console.WriteLine($"{r.Year}/{r.Month:D2} — {r.Total:C}");
        }

        // Task 11 - Search books by keyword
        public void SearchBooks(string keyword)
        {
            var books = _context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(keyword.ToLower()))
                .ToList();

            Console.WriteLine($"\n🔍 Search results for '{keyword}':");
            if (!books.Any())
                Console.WriteLine("No books found!");
            foreach (var b in books)
                Console.WriteLine($"{b.Title} — {b.Price:C}");
        }

        // Task 12 - Pagination
        public void GetBooksPaged(int pageNumber, int pageSize)
        {
            var books = _context.Books
                .AsNoTracking()
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine($"\n📄 Page {pageNumber} (Size: {pageSize}):");
            foreach (var b in books)
                Console.WriteLine($"{b.Title} — {b.Price:C}");
        }

        // Task 13 - Add, Update, Delete
        public void AddUpdateDelete()
        {
            // Add
            var newBook = new PaperbackBook
            {
                Title = "New Book",
                Price = 100,
                Stock = 10,
                AuthorId = 1,
                CategoryId = 1,
                Pages = 200,
                BookType = "Paperback"
            };
            _context.Books.Add(newBook);
            _context.SaveChanges();
            Console.WriteLine($"\n✅ Added: {newBook.Title} with ID: {newBook.Id}");

            // Update
            newBook.Price = 150;
            _context.SaveChanges();
            Console.WriteLine($"✅ Updated price to: {newBook.Price:C}");

            // Delete
            _context.Books.Remove(newBook);
            _context.SaveChanges();
            Console.WriteLine($"✅ Deleted: {newBook.Title}");
        }
    }
}