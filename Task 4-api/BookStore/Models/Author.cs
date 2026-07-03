using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new List<Book>();
    }
}