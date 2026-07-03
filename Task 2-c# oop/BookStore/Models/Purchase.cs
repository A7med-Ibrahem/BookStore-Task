namespace BookStoreApp.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public List<PurchaseItem> Items { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Total => Items.Sum(i => i.SubTotal);

        public Purchase(int id, Customer customer)
        {
            Id = id;
            Customer = customer;
            Items = new List<PurchaseItem>();
            PurchaseDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Purchase #{Id} | Customer: {Customer.FullName} | Date: {PurchaseDate:d} | Total: {Total:C}";
        }
    }
}