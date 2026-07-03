namespace BookStoreApp.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public decimal SubTotal => Quantity * PriceAtTime;

        public override string ToString()
        {
            return $"{Book?.Title} x{Quantity} @ {PriceAtTime:C} = {SubTotal:C}";
        }
    }
}