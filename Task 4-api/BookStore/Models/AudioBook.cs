using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class AudioBook : Book, IBookFormat
    {
        public string Narrator { get; set; } = string.Empty;
        public double Duration { get; set; }

        public AudioBook() { }

        public AudioBook(string title, string author, string category, decimal price, int stock, string narrator, double duration)
        {
            Title = title;
            Price = price;
            Stock = stock;
            Narrator = narrator;
            Duration = duration;
            BookType = "AudioBook";
        }

        public override string GetBookType() => "AudioBook";

        public string GetFormatDetails() => $"Narrator: {Narrator} | Duration: {Duration} hours";
    }
}