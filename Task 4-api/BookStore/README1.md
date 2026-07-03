# BookStore Console Application

A C# .NET console application for managing a bookstore.

---

## How to Run

1. Make sure you have **.NET 10** installed
2. Clone the repository
3. Open the solution in **Visual Studio**
4. Press **F5** to run

---

## Features

- Add, Remove, Search, and List books
- Support for 3 book formats: Paperback, EBook, AudioBook
- Register customers and record purchases
- Input validation with friendly error messages
- Filter books by category, author, or price range
- Statistics: total revenue, best-selling book, top customer
- Stock notifications when a book runs out

---

## Project Structure
BookStoreApp/
├── Models/
│   ├── Book.cs
│   ├── PaperbackBook.cs
│   ├── EBook.cs
│   ├── AudioBook.cs
│   ├── Customer.cs
│   ├── Purchase.cs
│   └── PurchaseItem.cs
├── Repositories/
│   ├── IRepository.cs
│   └── Repository.cs
├── Services/
│   └── BookStoreService.cs
├── Extensions/
│   └── BookExtensions.cs
├── Events/
│   └── StockNotifier.cs
├── Interfaces/
│   └── IBookFormat.cs
└── Program.cs
---

## Menu Options

| Option | Description |
|--------|-------------|
| 1 | Add a new book |
| 2 | Remove a book by ID |
| 3 | Search books by title or author |
| 4 | List all books |
| 5 | Register a new customer |
| 6 | List all customers |
| 7 | Make a purchase |
| 8 | Show statistics |
| 9 | Filter books |
| 0 | Exit |

---

## OOP Concepts Used

- **Inheritance** — PaperbackBook, EBook, AudioBook inherit from Book
- **Abstraction** — Abstract class Book and Interface IBookFormat
- **Encapsulation** — Private fields with public properties
- **Polymorphism** — GetBookType() differs per book type
- **Generics** — Repository<T> works with any entity
- **Events** — StockNotifier fires when book is out of stock
- **Extension Methods** — BookExtensions adds filter methods