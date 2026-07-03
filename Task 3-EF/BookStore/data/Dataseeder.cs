using BookStoreApp.Models;

namespace BookStoreApp.Data
{
    public static class DataSeeder
    {
        public static void Seed(BookStoreContext context)
        {
            if (context.Books.Any()) return;

            // Authors
            var authors = new List<Author>
            {
                new Author { AuthorName = "Robert Martin", Country = "USA" },
                new Author { AuthorName = "Jon Skeet", Country = "UK" },
                new Author { AuthorName = "James Clear", Country = "USA" },
                new Author { AuthorName = "David Thomas", Country = "UK" },
                new Author { AuthorName = "Ahmed Khaled", Country = "Egypt" },
                new Author { AuthorName = "Sara Ali", Country = "Saudi Arabia" },
            };
            context.Authors.AddRange(authors);

            // Categories
            var categories = new List<Category>
            {
                new Category { CategoryName = "Programming" },
                new Category { CategoryName = "Self-Help" },
                new Category { CategoryName = "Fiction" },
                new Category { CategoryName = "Science" },
                new Category { CategoryName = "History" },
                new Category { CategoryName = "Biography" },
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Books
            var books = new List<Book>
            {
                new PaperbackBook { Title = "Clean Code",               Price = 250, Stock = 10, AuthorId = 1, CategoryId = 1, Pages = 464,  BookType = "Paperback" },
                new EBook         { Title = "Learn C#",                 Price = 150, Stock = 20, AuthorId = 2, CategoryId = 1, FileFormat = "PDF", BookType = "EBook" },
                new AudioBook     { Title = "Atomic Habits",            Price = 200, Stock = 5,  AuthorId = 3, CategoryId = 2, Narrator = "John Smith", Duration = 5.5, BookType = "AudioBook" },
                new PaperbackBook { Title = "The Pragmatic Programmer", Price = 300, Stock = 8,  AuthorId = 4, CategoryId = 1, Pages = 352,  BookType = "Paperback" },
                new PaperbackBook { Title = "Desert Storm",             Price = 130, Stock = 15, AuthorId = 5, CategoryId = 3, Pages = 280,  BookType = "Paperback" },
                new EBook         { Title = "Python Basics",            Price = 180, Stock = 25, AuthorId = 2, CategoryId = 1, FileFormat = "EPUB", BookType = "EBook" },
                new PaperbackBook { Title = "World War II",             Price = 160, Stock = 12, AuthorId = 4, CategoryId = 5, Pages = 400,  BookType = "Paperback" },
                new AudioBook     { Title = "Think Better",             Price = 90,  Stock = 30, AuthorId = 6, CategoryId = 2, Narrator = "Sara Ali", Duration = 3.5, BookType = "AudioBook" },
                new PaperbackBook { Title = "Napoleon Life",            Price = 170, Stock = 7,  AuthorId = 5, CategoryId = 6, Pages = 320,  BookType = "Paperback" },
                new EBook         { Title = "Data Structures",          Price = 220, Stock = 18, AuthorId = 1, CategoryId = 1, FileFormat = "PDF", BookType = "EBook" },
                new PaperbackBook { Title = "The Lost City",            Price = 120, Stock = 22, AuthorId = 5, CategoryId = 3, Pages = 250,  BookType = "Paperback" },
                new EBook         { Title = "Web Development",          Price = 210, Stock = 14, AuthorId = 4, CategoryId = 1, FileFormat = "EPUB", BookType = "EBook" },
            };
            context.Books.AddRange(books);

            // Customers
            var customers = new List<Customer>
            {
                new Customer { FullName = "Ahmed Mohamed", Email = "ahmed@email.com", City = "Cairo" },
                new Customer { FullName = "Sara Hassan",   Email = "sara@email.com",  City = "Alexandria" },
                new Customer { FullName = "Mohamed Ali",   Email = "mohamed@email.com", City = "Cairo" },
                new Customer { FullName = "Nour Ibrahim",  Email = "nour@email.com",  City = "Giza" },
                new Customer { FullName = "Tarek Samir",   Email = "tarek@email.com", City = "Aswan" },
            };
            context.Customers.AddRange(customers);
            context.SaveChanges();

            // Purchases
            var purchases = new List<Purchase>
            {
                new Purchase { CustomerId = 1, PurchaseDate = new DateTime(2024, 1, 15) },
                new Purchase { CustomerId = 2, PurchaseDate = new DateTime(2024, 2, 20) },
                new Purchase { CustomerId = 1, PurchaseDate = new DateTime(2024, 3, 10) },
                new Purchase { CustomerId = 3, PurchaseDate = new DateTime(2024, 4, 5)  },
            };
            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            // PurchaseItems
            var items = new List<PurchaseItem>
            {
                new PurchaseItem { PurchaseId = 1, BookId = 1, Quantity = 2, PriceAtTime = 250 },
                new PurchaseItem { PurchaseId = 1, BookId = 2, Quantity = 1, PriceAtTime = 150 },
                new PurchaseItem { PurchaseId = 2, BookId = 3, Quantity = 1, PriceAtTime = 200 },
                new PurchaseItem { PurchaseId = 2, BookId = 4, Quantity = 2, PriceAtTime = 300 },
                new PurchaseItem { PurchaseId = 3, BookId = 1, Quantity = 1, PriceAtTime = 250 },
                new PurchaseItem { PurchaseId = 3, BookId = 5, Quantity = 3, PriceAtTime = 130 },
                new PurchaseItem { PurchaseId = 4, BookId = 6, Quantity = 2, PriceAtTime = 180 },
            };
            context.PurchaseItems.AddRange(items);
            context.SaveChanges();
        }
    }
}