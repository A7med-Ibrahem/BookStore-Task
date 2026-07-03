USE BookStore;

CREATE TABLE Categories (
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

CREATE TABLE Books (
    BookID       INT IDENTITY(1,1) PRIMARY KEY,
    Title        NVARCHAR(200) NOT NULL,
    Price        DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    Stock        INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    AuthorID     INT REFERENCES Authors(AuthorID),
    CategoryID   INT REFERENCES Categories(CategoryID)
);

CREATE TABLE Orders (
    OrderID      INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID   INT REFERENCES Customers(CustomerID),
    OrderDate    DATETIME DEFAULT GETDATE()
);

CREATE TABLE OrderItems (
    OrderItemID  INT IDENTITY(1,1) PRIMARY KEY,
    OrderID      INT REFERENCES Orders(OrderID),
    BookID       INT REFERENCES Books(BookID),
    Quantity     INT NOT NULL CHECK (Quantity > 0),
    PriceAtTime  DECIMAL(10,2) NOT NULL
);