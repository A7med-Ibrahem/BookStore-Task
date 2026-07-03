using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class PaperbackBook : Book, IBookFormat
    {
        public int Pages { get; set; }

        public PaperbackBook(int id, string title, string author, string category, decimal price, int stock, int pages)
            : base(id, title, author, category, price, stock)
        {
            Pages = pages;
        }

        public override string GetBookType()
        {
            return "Paperback";
        }

        public string GetFormatDetails()
        {
            return $"Pages: {Pages}";
        }
    }
}