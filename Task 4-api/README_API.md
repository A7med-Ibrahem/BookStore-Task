# BookStore Web API

A .NET 10 Web API for an online bookstore with JWT authentication, role-based authorization, and Swagger documentation.

---

## Requirements

- .NET 10
- SQL Server Express
- Visual Studio 2022+

---

## How to Run

1. Clone the repository
2. Open the solution in Visual Studio
3. Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=BookStoreEF;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

4. Open **Package Manager Console** and run:

```
Update-Database
```

5. Press **F5** to run the application
6. Open Swagger at: `https://localhost:7209/swagger`

---

## How to Register the First Admin

After running the app, register a normal user first:

**Step 1** — Register via Swagger:
```json
POST /api/auth/register
{
  "fullName": "Admin User",
  "email": "admin@bookstore.com",
  "password": "Admin@123",
  "city": "Cairo"
}
```

**Step 2** — Open SQL Server Management Studio and run:
```sql
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'admin@bookstore.com'
AND r.Name = 'Admin'
```

**Step 3** — Login to get your Admin token:
```json
POST /api/auth/login
{
  "email": "admin@bookstore.com",
  "password": "Admin@123"
}
```

---

## How to Test the API

### Using Swagger

1. Open `https://localhost:7209/swagger`
2. Register via `POST /api/auth/register`
3. Login via `POST /api/auth/login` and copy the token
4. Click the **Authorize** button at the top right
5. Enter: `Bearer {your_token_here}`
6. Click **Authorize** then **Close**
7. Now you can test all protected endpoints

---

## API Endpoints

### Auth
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /api/auth/register | Register new user | ❌ |
| POST | /api/auth/login | Login and get JWT token | ❌ |

### Books
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /api/books | List all books (with pagination & filter) | ❌ |
| GET | /api/books/{id} | Get book details | ❌ |
| POST | /api/books | Add new book | ✅ Admin |
| PUT | /api/books/{id} | Update book | ✅ Admin |
| DELETE | /api/books/{id} | Delete book | ✅ Admin |

### Books Filter Query Parameters
```
GET /api/books?keyword=clean&category=Programming&author=Robert&minPrice=100&maxPrice=300&page=1&pageSize=10
```

### Categories
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /api/categories | List all categories | ❌ |
| GET | /api/categories/{id} | Get category details | ❌ |
| POST | /api/categories | Add new category | ✅ Admin |
| PUT | /api/categories/{id} | Update category | ✅ Admin |
| DELETE | /api/categories/{id} | Delete category | ✅ Admin |

### Authors
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /api/authors | List all authors | ❌ |
| GET | /api/authors/{id} | Get author details | ❌ |
| POST | /api/authors | Add new author | ✅ Admin |
| PUT | /api/authors/{id} | Update author | ✅ Admin |
| DELETE | /api/authors/{id} | Delete author | ✅ Admin |

### Orders
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /api/orders | Place a new order | ✅ Customer/Admin |
| GET | /api/orders | View orders (Customer sees own, Admin sees all) | ✅ Customer/Admin |
| GET | /api/orders/{id} | View order details | ✅ Customer/Admin |

---

## Sample Screenshots

### Swagger UI
![Swagger UI](screenshots/swagger.png)

### Register
![Register](screenshots/register.png)

### Login
![Login](screenshots/login.png)

---

## Project Structure

```
BookStoreApp/
├── Controllers/
│   ├── AuthController.cs
│   ├── BooksController.cs
│   ├── CategoriesController.cs
│   ├── AuthorsController.cs
│   └── OrdersController.cs
├── DTOs/
│   ├── AuthDTOs.cs
│   ├── BookDTOs.cs
│   ├── OrderDTOs.cs
│   └── CategoryAuthorDTOs.cs
├── Data/
│   ├── BookStoreContext.cs
│   ├── DataSeeder.cs
│   └── BookStoreQueries.cs
├── Helpers/
│   └── JwtHelper.cs
├── Middleware/
│   └── ErrorHandlingMiddleware.cs
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
├── Migrations/
├── appsettings.json
└── Program.cs
```

---

## Features

- ✅ JWT Authentication
- ✅ Role-based Authorization (Admin & Customer)
- ✅ Swagger UI with JWT Support
- ✅ Global Error Handling Middleware
- ✅ Serilog Logging
- ✅ CORS Configuration
- ✅ Input Validation with Data Annotations
- ✅ DTOs (database entities never exposed)
- ✅ Pagination, Search & Filter for books
- ✅ Dependency Injection
- ✅ Code First with EF Core Migrations
