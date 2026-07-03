using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class EBook : Book, IBookFormat
    {
        public string FileFormat { get; set; }

        public EBook(int id, string title, string author, string category, decimal price, int stock, string fileFormat)
            : base(id, title, author, category, price, stock)
        {
            FileFormat = fileFormat;
        }

        public override string GetBookType()
        {
            return "EBook";
        }

        public string GetFormatDetails()
        {
            return $"File Format: {FileFormat}";
        }
    }
}