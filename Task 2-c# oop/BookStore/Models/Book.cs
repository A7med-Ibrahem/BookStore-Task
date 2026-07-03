namespace BookStoreApp.Models
{
    public abstract class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        protected Book(int id, string title, string author, string category, decimal price, int stock)
        {
            Id = id;
            Title = title;
            Author = author;
            Category = category;
            Price = price;
            Stock = stock;
        }

        public abstract string GetBookType();

        public override string ToString()
        {
            return $"[{GetBookType()}] {Title} by {Author} | Category: {Category} | Price: {Price:C} | Stock: {Stock}";
        }
    }
}