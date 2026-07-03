using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public abstract class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int Stock { get; set; }

        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public string BookType { get; set; } = string.Empty;

        public abstract string GetBookType();

        public override string ToString()
        {
            return $"[{GetBookType()}] {Title} by {Author?.AuthorName} | Category: {Category?.CategoryName} | Price: {Price:C} | Stock: {Stock}";
        }
    }
}