namespace BookStoreApp.Models
{
    public class PurchaseItem
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
        public decimal SubTotal => Quantity * PriceAtTime;

        public PurchaseItem(Book book, int quantity)
        {
            Book = book;
            Quantity = quantity;
            PriceAtTime = book.Price;
        }

        public override string ToString()
        {
            return $"{Book.Title} x{Quantity} @ {PriceAtTime:C} = {SubTotal:C}";
        }
    }
}