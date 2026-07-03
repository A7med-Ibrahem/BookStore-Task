using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Models;

namespace BookStoreApp.Data
{
    public class BookStoreContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }

        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Book>()
                .ToTable(t => t.HasCheckConstraint("CK_Book_Price", "Price > 0"));

            modelBuilder.Entity<Book>()
                .ToTable(t => t.HasCheckConstraint("CK_Book_Stock", "Stock >= 0"));

            // Book Type discriminator
            modelBuilder.Entity<Book>()
                .HasDiscriminator(b => b.BookType)
                .HasValue<PaperbackBook>("Paperback")
                .HasValue<EBook>("EBook")
                .HasValue<AudioBook>("AudioBook");

            // Customer
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // PurchaseItem
            modelBuilder.Entity<PurchaseItem>()
                .Property(p => p.PriceAtTime)
                .HasColumnType("decimal(10,2)");

            // Book -> Author (no cascade delete)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book -> Category (no cascade delete)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}