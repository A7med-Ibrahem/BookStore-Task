CREATE PROCEDURE GetCustomerPurchases
    @CustomerID INT
AS
BEGIN
    SELECT O.OrderID,
           O.OrderDate,
           B.Title                        AS BookTitle,
           OI.Quantity,
           OI.PriceAtTime,
           OI.Quantity * OI.PriceAtTime   AS SubTotal
    FROM Orders O
    JOIN OrderItems OI ON O.OrderID    = OI.OrderID
    JOIN Books B       ON OI.BookID    = B.BookID
    WHERE O.CustomerID = @CustomerID

    SELECT SUM(OI.Quantity * OI.PriceAtTime) AS TotalAmount
    FROM Orders O
    JOIN OrderItems OI ON O.OrderID = OI.OrderID
    WHERE O.CustomerID = @CustomerID
END;

EXEC GetCustomerPurchases @CustomerID = 1;