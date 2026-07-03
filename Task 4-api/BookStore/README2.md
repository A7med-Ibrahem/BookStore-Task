# BookStore Data Layer — EF Core

A .NET 10 console application using Entity Framework Core with SQL Server.

---

## Requirements

- .NET 10
- SQL Server Express
- Visual Studio 2022+

---

## How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Open **Package Manager Console** and run:
4. Press **F5** to run the application
5. Sample data will be seeded automatically on first run

---

## Features

| Option | Description |
|--------|-------------|
| 1  | Add a new book |
| 2  | Remove a book |
| 3  | Search books |
| 4  | List all books |
| 5  | Register a new customer |
| 6  | List all customers |
| 7  | Make a purchase |
| 8  | Show statistics |
| 9  | Filter books |
| 10 | List books with category and author (EF) |
| 11 | Top 5 best-selling books (EF) |
| 12 | Customers with purchase count (EF) |
| 13 | Categories with more than 5 books (EF) |
| 14 | Books above average price (EF) |
| 15 | Customers with no purchases (EF) |
| 16 | Revenue by month (EF) |
| 17 | Search books by keyword (EF) |
| 18 | Pagination (EF) |
| 19 | Add, Update, Delete demo (EF) |

---

## EF Core Concepts Used

- **Code First** — Database generated from C# models
- **Migrations** — Schema versioning
- **AsNoTracking** — Efficient read-only queries
- **Include** — Eager loading to fix N+1 problem
- **DbContext** — Database connection and configuration
- **Data Seeding** — Sample data on first run

---

## Project Structure
BookStoreApp/
├── Data/
│   ├── BookStoreContext.cs
│   ├── BookStoreQueries.cs
│   └── DataSeeder.cs
├── Migrations/
├── Models/
│   ├── Book.cs
│   ├── PaperbackBook.cs
│   ├── EBook.cs
│   ├── AudioBook.cs
│   ├── Author.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── Purchase.cs
│   └── PurchaseItem.cs
├── Repositories/
├── Services/
├── Extensions/
├── Events/
├── Interfaces/
└── Program.cs