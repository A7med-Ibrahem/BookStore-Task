using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemResponseDTO> Items { get; set; } = new();
    }

    public class OrderItemResponseDTO
    {
        public string BookTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public decimal SubTotal { get; set; }
    }
}