using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace CPS2
{
    public class AppDbContext : DbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=Books;Username=postgres;Password=485327");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Series>()
                .HasOne(s => s.Genre)
                .WithMany(g => g.Series)
                .HasForeignKey(s => s.GenreId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Series)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.SeriesId);
        }
    }
}