using BookStoreApp.Models;

namespace BookStoreApp.Extensions
{
    public static class BookExtensions
    {
        public static List<Book> FilterByCategory(this List<Book> books, string category)
        {
            return books.Where(b => b.Category.ToLower() == category.ToLower()).ToList();
        }

        public static List<Book> FilterByAuthor(this List<Book> books, string author)
        {
            return books.Where(b => b.Author.ToLower().Contains(author.ToLower())).ToList();
        }

        public static List<Book> FilterByPriceRange(this List<Book> books, decimal min, decimal max)
        {
            return books.Where(b => b.Price >= min && b.Price <= max).ToList();
        }

        public static void ApplyDiscount(this List<Book> books, Func<Book, bool> rule, decimal percentage)
        {
            foreach (var book in books.Where(rule))
            {
                book.Price -= book.Price * percentage / 100;
            }
        }
    }
}