using BookStoreApp.Models;
using BookStoreApp.Repositories;
using BookStoreApp.Events;
using BookStoreApp.Extensions;

namespace BookStoreApp.Services
{
    public class BookStoreService
    {
        private readonly Repository<Book> _books;
        private readonly Repository<Customer> _customers;
        private readonly Repository<Purchase> _purchases;
        private readonly StockNotifier _stockNotifier;
        private int _bookIdCounter = 1;
        private int _customerIdCounter = 1;
        private int _purchaseIdCounter = 1;

        public BookStoreService()
        {
            _books = new Repository<Book>(b => b.Id);
            _customers = new Repository<Customer>(c => c.Id);
            _purchases = new Repository<Purchase>(p => p.Id);
            _stockNotifier = new StockNotifier();
            _stockNotifier.OnOutOfStock += (book) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n⚠️  WARNING: '{book.Title}' is now OUT OF STOCK!");
                Console.ResetColor();
            };
        }

        // ===== BOOKS =====
        public void AddBook(Book book)
        {
            book.Id = _bookIdCounter++;
            _books.Add(book);
            Console.WriteLine($"✅ Book '{book.Title}' added successfully!");
        }

        public void RemoveBook(int id)
        {
            var book = _books.GetById(id);
            if (book == null)
            {
                Console.WriteLine("❌ Book not found!");
                return;
            }
            _books.Remove(id);
            Console.WriteLine($"✅ Book '{book.Title}' removed successfully!");
        }

        public void SearchBooks(string keyword)
        {
            var results = _books.GetAll()
                .Where(b => b.Title.ToLower().Contains(keyword.ToLower()))
                .ToList();

            if (!results.Any())
            {
                Console.WriteLine("❌ No books found!");
                return;
            }
            results.ForEach(b => Console.WriteLine(b));
        }

        public void ListAllBooks()
        {
            var books = _books.GetAll();
            if (!books.Any())
            {
                Console.WriteLine("❌ No books available!");
                return;
            }
            books.ForEach(b => Console.WriteLine(b));
        }

        // ===== CUSTOMERS =====
        public void RegisterCustomer(string fullName, string email, string city)
        {
            var exists = _customers.GetAll().Any(c => c.Email.ToLower() == email.ToLower());
            if (exists)
            {
                Console.WriteLine("❌ Email already registered!");
                return;
            }
            var customer = new Customer
            {
                Id = _customerIdCounter++,
                FullName = fullName,
                Email = email,
                City = city
            };
            _customers.Add(customer);
            Console.WriteLine($"✅ Customer '{fullName}' registered successfully!");
        }

        public void ListAllCustomers()
        {
            var customers = _customers.GetAll();
            if (!customers.Any())
            {
                Console.WriteLine("❌ No customers registered!");
                return;
            }
            customers.ForEach(c => Console.WriteLine(c));
        }

        // ===== PURCHASES =====
        public void MakePurchase(int customerId, List<(int bookId, int quantity)> items)
        {
            var customer = _customers.GetById(customerId);
            if (customer == null)
            {
                Console.WriteLine("❌ Customer not found!");
                return;
            }

            var purchase = new Purchase
            {
                Id = _purchaseIdCounter++,
                CustomerId = customerId,
                Customer = customer
            };

            foreach (var (bookId, quantity) in items)
            {
                var book = _books.GetById(bookId);
                if (book == null)
                {
                    Console.WriteLine($"❌ Book ID {bookId} not found!");
                    return;
                }
                if (book.Stock < quantity)
                {
                    Console.WriteLine($"❌ Not enough stock for '{book.Title}'! Available: {book.Stock}");
                    return;
                }
                book.Stock -= quantity;
                purchase.Items.Add(new PurchaseItem
                {
                    BookId = bookId,
                    Book = book,
                    Quantity = quantity,
                    PriceAtTime = book.Price
                });
                _stockNotifier.CheckStock(book);
            }

            customer.Purchases.Add(purchase);
            _purchases.Add(purchase);
            Console.WriteLine($"✅ Purchase completed! Total: {purchase.Total:C}");
        }

        // ===== STATISTICS =====
        public void ShowStatistics()
        {
            var purchases = _purchases.GetAll();
            if (!purchases.Any())
            {
                Console.WriteLine("❌ No purchases yet!");
                return;
            }

            decimal totalRevenue = purchases.Sum(p => p.Total);

            var bestBook = purchases
                .SelectMany(p => p.Items)
                .GroupBy(i => i.Book!.Title)
                .OrderByDescending(g => g.Sum(i => i.Quantity))
                .FirstOrDefault();

            var topCustomer = purchases
                .GroupBy(p => p.Customer!.FullName)
                .OrderByDescending(g => g.Sum(p => p.Total))
                .FirstOrDefault();

            Console.WriteLine($"\n📊 Statistics:");
            Console.WriteLine($"💰 Total Revenue: {totalRevenue:C}");
            Console.WriteLine($"📚 Best Selling Book: {bestBook?.Key ?? "N/A"}");
            Console.WriteLine($"👤 Top Customer: {topCustomer?.Key ?? "N/A"}");
        }

        // ===== FILTERS =====
        public void FilterBooks(string type, string value)
        {
            var books = _books.GetAll();
            List<Book> results = new List<Book>();

            switch (type)
            {
                case "category":
                    results = books.FilterByCategory(value);
                    break;
                case "author":
                    results = books.FilterByAuthor(value);
                    break;
                case "price":
                    var parts = value.Split('-');
                    results = books.FilterByPriceRange(decimal.Parse(parts[0]), decimal.Parse(parts[1]));
                    break;
            }

            if (!results.Any())
            {
                Console.WriteLine("❌ No books found!");
                return;
            }
            results.ForEach(b => Console.WriteLine(b));
        }
    }
}