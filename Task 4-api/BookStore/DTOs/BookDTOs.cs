using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.DTOs
{
    public class BookResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string BookType { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CreateBookDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required]
        public string BookType { get; set; } = string.Empty;

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? Pages { get; set; }
        public string? FileFormat { get; set; }
        public string? Narrator { get; set; }
        public double? Duration { get; set; }
    }

    public class UpdateBookDTO
    {
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
    }

    public class BookFilterDTO
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? Author { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}