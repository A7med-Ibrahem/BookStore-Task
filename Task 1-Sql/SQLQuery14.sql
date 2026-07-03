USE BookStore;

SELECT B.Title        AS BookTitle,
       C.CategoryName AS Category,
       A.AuthorName   AS Author
FROM Books B
JOIN Categories C ON B.CategoryID = C.CategoryID
JOIN Authors    A ON B.AuthorID   = A.AuthorID;