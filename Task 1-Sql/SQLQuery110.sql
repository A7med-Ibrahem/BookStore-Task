USE BookStore;

SELECT 'Customers'  AS TableName, COUNT(*) AS TotalRows FROM Customers  UNION ALL
SELECT 'Authors',                 COUNT(*)              FROM Authors     UNION ALL
SELECT 'Categories',              COUNT(*)              FROM Categories  UNION ALL
SELECT 'Books',                   COUNT(*)              FROM Books       UNION ALL
SELECT 'Orders',                  COUNT(*)              FROM Orders      UNION ALL
SELECT 'OrderItems',              COUNT(*)              FROM OrderItems;