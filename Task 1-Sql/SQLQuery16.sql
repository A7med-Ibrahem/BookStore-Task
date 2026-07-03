USE BookStore;

SELECT TOP 5
       B.Title          AS BookTitle,
       SUM(OI.Quantity) AS TotalSold
FROM Books B
JOIN OrderItems OI ON B.BookID = OI.BookID
GROUP BY B.BookID, B.Title
ORDER BY TotalSold DESC;

USE BookStore;

SELECT * FROM OrderItems;