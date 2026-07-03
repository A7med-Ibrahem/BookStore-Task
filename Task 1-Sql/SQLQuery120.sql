USE BookStore;

SELECT C.CategoryName   AS Category,
       COUNT(B.BookID)  AS NumberOfBooks
FROM Categories C
JOIN Books B ON C.CategoryID = B.CategoryID
GROUP BY C.CategoryID, C.CategoryName
HAVING COUNT(B.BookID) > 5;