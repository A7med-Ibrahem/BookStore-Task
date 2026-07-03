using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.DTOs
{
    // Category DTOs
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    public class CreateCategoryDTO
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class UpdateCategoryDTO
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }

    // Author DTOs
    public class AuthorResponseDTO
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    public class CreateAuthorDTO
    {
        [Required]
        public string AuthorName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class UpdateAuthorDTO
    {
        [Required]
        public string AuthorName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}