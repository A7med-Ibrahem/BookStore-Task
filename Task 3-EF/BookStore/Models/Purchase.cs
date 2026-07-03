namespace BookStoreApp.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public decimal Total => Items.Sum(i => i.SubTotal);

        public override string ToString()
        {
            return $"Purchase #{Id} | Customer: {Customer?.FullName} | Date: {PurchaseDate:d} | Total: {Total:C}";
        }
    }
}