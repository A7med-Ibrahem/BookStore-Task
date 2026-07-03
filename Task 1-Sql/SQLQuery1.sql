USE BookStore;

CREATE TABLE Customers (
    CustomerID   INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    City         NVARCHAR(50),
    CreatedAt    DATETIME DEFAULT GETDATE()
);

CREATE TABLE Authors (
    AuthorID    INT IDENTITY(1,1) PRIMARY KEY,
    AuthorName  NVARCHAR(100) NOT NULL,
    Country     NVARCHAR(50)
);

