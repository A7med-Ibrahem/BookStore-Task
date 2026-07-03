using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public override string ToString()
        {
            return $"[{Id}] {FullName} | Email: {Email} | City: {City} | Purchases: {Purchases.Count}";
        }
    }
}