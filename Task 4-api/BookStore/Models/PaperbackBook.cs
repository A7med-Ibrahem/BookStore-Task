using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class PaperbackBook : Book, IBookFormat
    {
        public int Pages { get; set; }

        public PaperbackBook() { }

        public PaperbackBook(string title, string author, string category, decimal price, int stock, int pages)
        {
            Title = title;
            Pages = pages;
            Price = price;
            Stock = stock;
            BookType = "Paperback";
        }

        public override string GetBookType() => "Paperback";

        public string GetFormatDetails() => $"Pages: {Pages}";
    }
}