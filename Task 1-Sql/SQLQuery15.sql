USE BookStore;

SELECT C.FullName        AS CustomerName,
       COUNT(O.OrderID)  AS NumberOfPurchases
FROM Customers C
LEFT JOIN Orders O ON C.CustomerID = O.CustomerID
GROUP BY C.CustomerID, C.FullName
ORDER BY NumberOfPurchases DESC;