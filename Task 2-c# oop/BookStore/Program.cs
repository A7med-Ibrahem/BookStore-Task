using BookStoreApp.Models;
using BookStoreApp.Services;

var service = new BookStoreService();

// Sample Data
service.AddBook(new PaperbackBook(0, "Clean Code", "Robert Martin", "Programming", 250, 5, 464));
service.AddBook(new EBook(0, "Learn C#", "Jon Skeet", "Programming", 150, 10, "PDF"));
service.AddBook(new AudioBook(0, "Atomic Habits", "James Clear", "Self-Help", 200, 3, "John Smith", 5.5));
service.AddBook(new PaperbackBook(0, "The Pragmatic Programmer", "David Thomas", "Programming", 300, 7, 352));
service.RegisterCustomer("Ahmed Mohamed", "ahmed@email.com", "Cairo");
service.RegisterCustomer("Sara Hassan", "sara@email.com", "Alexandria");

while (true)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╔══════════════════════════════╗");
    Console.WriteLine("║      📚 BookStore App         ║");
    Console.WriteLine("╠══════════════════════════════╣");
    Console.WriteLine("║  1. Add Book                  ║");
    Console.WriteLine("║  2. Remove Book               ║");
    Console.WriteLine("║  3. Search Books              ║");
    Console.WriteLine("║  4. List All Books            ║");
    Console.WriteLine("║  5. Register Customer         ║");
    Console.WriteLine("║  6. List All Customers        ║");
    Console.WriteLine("║  7. Make Purchase             ║");
    Console.WriteLine("║  8. Show Statistics           ║");
    Console.WriteLine("║  9. Filter Books              ║");
    Console.WriteLine("║  0. Exit                      ║");
    Console.WriteLine("╚══════════════════════════════╝");
    Console.ResetColor();
    Console.Write("\nChoose an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddBook(service);
            break;
        case "2":
            RemoveBook(service);
            break;
        case "3":
            SearchBooks(service);
            break;
        case "4":
            service.ListAllBooks();
            break;
        case "5":
            RegisterCustomer(service);
            break;
        case "6":
            service.ListAllCustomers();
            break;
        case "7":
            MakePurchase(service);
            break;
        case "8":
            service.ShowStatistics();
            break;
        case "9":
            FilterBooks(service);
            break;
        case "0":
            Console.WriteLine("Goodbye! 👋");
            return;
        default:
            Console.WriteLine("❌ Invalid option!");
            break;
    }

    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

// ===== METHODS =====

void AddBook(BookStoreService service)
{
    Console.WriteLine("\n📚 Add New Book");
    Console.WriteLine("1. Paperback\n2. EBook\n3. AudioBook");
    Console.Write("Choose type: ");
    var type = Console.ReadLine();

    Console.Write("Title: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("❌ Title cannot be empty!"); return; }

    Console.Write("Author: ");
    var author = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(author)) { Console.WriteLine("❌ Author cannot be empty!"); return; }

    Console.Write("Category: ");
    var category = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(category)) { Console.WriteLine("❌ Category cannot be empty!"); return; }

    Console.Write("Price: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
    { Console.WriteLine("❌ Invalid price!"); return; }

    Console.Write("Stock: ");
    if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
    { Console.WriteLine("❌ Invalid stock!"); return; }

    switch (type)
    {
        case "1":
            Console.Write("Pages: ");
            if (!int.TryParse(Console.ReadLine(), out int pages) || pages <= 0)
            { Console.WriteLine("❌ Invalid pages!"); return; }
            service.AddBook(new PaperbackBook(0, title, author, category, price, stock, pages));
            break;
        case "2":
            Console.Write("File Format (PDF/EPUB): ");
            var format = Console.ReadLine() ?? "PDF";
            service.AddBook(new EBook(0, title, author, category, price, stock, format));
            break;
        case "3":
            Console.Write("Narrator: ");
            var narrator = Console.ReadLine() ?? "";
            Console.Write("Duration (hours): ");
            if (!double.TryParse(Console.ReadLine(), out double duration) || duration <= 0)
            { Console.WriteLine("❌ Invalid duration!"); return; }
            service.AddBook(new AudioBook(0, title, author, category, price, stock, narrator, duration));
            break;
        default:
            Console.WriteLine("❌ Invalid type!");
            break;
    }
}

void RemoveBook(BookStoreService service)
{
    Console.Write("\nEnter Book ID to remove: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    { Console.WriteLine("❌ Invalid ID!"); return; }
    service.RemoveBook(id);
}

void SearchBooks(BookStoreService service)
{
    Console.Write("\nEnter keyword to search: ");
    var keyword = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(keyword)) { Console.WriteLine("❌ Keyword cannot be empty!"); return; }
    service.SearchBooks(keyword);
}

void RegisterCustomer(BookStoreService service)
{
    Console.WriteLine("\n👤 Register New Customer");
    Console.Write("Full Name: ");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("❌ Name cannot be empty!"); return; }

    Console.Write("Email: ");
    var email = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
    { Console.WriteLine("❌ Invalid email!"); return; }

    Console.Write("City: ");
    var city = Console.ReadLine() ?? "";
    service.RegisterCustomer(name, email, city);
}

void MakePurchase(BookStoreService service)
{
    Console.Write("\nEnter Customer ID: ");
    if (!int.TryParse(Console.ReadLine(), out int customerId))
    { Console.WriteLine("❌ Invalid ID!"); return; }

    var items = new List<(int, int)>();
    while (true)
    {
        Console.Write("Enter Book ID (or 0 to finish): ");
        if (!int.TryParse(Console.ReadLine(), out int bookId)) { Console.WriteLine("❌ Invalid ID!"); continue; }
        if (bookId == 0) break;

        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
        { Console.WriteLine("❌ Invalid quantity!"); continue; }

        items.Add((bookId, qty));
    }

    if (!items.Any()) { Console.WriteLine("❌ No items added!"); return; }
    service.MakePurchase(customerId, items);
}

void FilterBooks(BookStoreService service)
{
    Console.WriteLine("\n🔍 Filter Books");
    Console.WriteLine("1. By Category\n2. By Author\n3. By Price Range");
    Console.Write("Choose: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Category: ");
            var category = Console.ReadLine() ?? "";
            service.FilterBooks("category", category);
            break;
        case "2":
            Console.Write("Author: ");
            var author = Console.ReadLine() ?? "";
            service.FilterBooks("author", author);
            break;
        case "3":
            Console.Write("Min Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal min))
            { Console.WriteLine("❌ Invalid price!"); return; }
            Console.Write("Max Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal max))
            { Console.WriteLine("❌ Invalid price!"); return; }
            service.FilterBooks("price", $"{min}-{max}");
            break;
        default:
            Console.WriteLine("❌ Invalid option!");
            break;
    }
}