using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new List<Book>();
    }
}