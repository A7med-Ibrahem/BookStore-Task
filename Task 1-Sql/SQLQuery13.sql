USE BookStore;

SELECT UPPER(B.Title) AS BookTitle, 
       LOWER(A.AuthorName) AS AuthorName
FROM Books B
JOIN Authors A ON B.AuthorID = A.AuthorID;