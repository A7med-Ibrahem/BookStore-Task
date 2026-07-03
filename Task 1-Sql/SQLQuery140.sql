USE BookStore;

CREATE VIEW BookDetails AS
SELECT B.Title          AS BookTitle,
       C.CategoryName   AS Category,
       A.AuthorName     AS Author,
       B.Price          AS Price
FROM Books B
JOIN Categories C ON B.CategoryID = C.CategoryID
JOIN Authors    A ON B.AuthorID   = A.AuthorID;