using Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Library.Settings
{
    public class LibraryDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> AuthorBooks { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookAuthor>().HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder
                .Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(ba => ba.AuthorBooks)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder
                .Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ba => ba.AuthorId);

            #region books seed
            Book book1 = new Book() { Id = 1, Name = "Crime and Punishment", Quantity = 15, Language = Enums.Language.RUSSIAN, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
            modelBuilder.Entity<Book>().HasData(
               book1
               );
            #endregion

            #region authors seed
            Author author1 = new Author() { Id = 1, FirstName = "Fyodor", LastName = "Dostoevsky", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow };
            modelBuilder.Entity<Author>().HasData(
               author1);
            #endregion

            #region bookauthor seed
            modelBuilder.Entity<BookAuthor>().HasData(
               new BookAuthor() { AuthorId = 1, BookId = 1 });
            #endregion

        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is EntityBase && (
                        e.State == EntityState.Added
                || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (EntityBase)entityEntry.Entity;
                DateTime dateTime = DateTime.UtcNow;
                entity.ModifiedDate = dateTime;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedDate = dateTime;
                }
            }

            return base.SaveChanges();
        }
    }
}
