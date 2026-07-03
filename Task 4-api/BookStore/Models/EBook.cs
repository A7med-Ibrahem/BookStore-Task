using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class EBook : Book, IBookFormat
    {
        public string FileFormat { get; set; } = string.Empty;

        public EBook() { }

        public EBook(string title, string author, string category, decimal price, int stock, string fileFormat)
        {
            Title = title;
            Price = price;
            Stock = stock;
            FileFormat = fileFormat;
            BookType = "EBook";
        }

        public override string GetBookType() => "EBook";

        public string GetFormatDetails() => $"File Format: {FileFormat}";
    }
}