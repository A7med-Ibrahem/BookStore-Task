using BookStoreApp.Models;

namespace BookStoreApp.Events
{
    public class StockNotifier
    {
        public event Action<Book>? OnOutOfStock;

        public void CheckStock(Book book)
        {
            if (book.Stock == 0)
            {
                OnOutOfStock?.Invoke(book);
            }
        }
    }
}