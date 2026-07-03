using BookStoreApp.Models;

namespace BookStoreApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public List<Purchase> Purchases { get; set; }

        public Customer(int id, string fullName, string email, string city)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            City = city;
            Purchases = new List<Purchase>();
        }

        public override string ToString()
        {
            return $"[{Id}] {FullName} | Email: {Email} | City: {City} | Purchases: {Purchases.Count}";
        }
    }
}