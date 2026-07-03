using BookStoreApp.Interfaces;

namespace BookStoreApp.Models
{
    public class AudioBook : Book, IBookFormat
    {
        public string Narrator { get; set; }
        public double Duration { get; set; }

        public AudioBook(int id, string title, string author, string category, decimal price, int stock, string narrator, double duration)
            : base(id, title, author, category, price, stock)
        {
            Narrator = narrator;
            Duration = duration;
        }

        public override string GetBookType()
        {
            return "AudioBook";
        }

        public string GetFormatDetails()
        {
            return $"Narrator: {Narrator} | Duration: {Duration} hours";
        }
    }
}