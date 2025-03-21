using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseNpgsql("Host=localhost;Database=Books;Username=postgres;Password=485327;Include Error Detail=true");
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>()
                .HasMany(g => g.Series)
                .WithOne(s => s.Genre)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Series>()
                .HasMany(s => s.Books)
                .WithOne(b => b.Series)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Series)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.SeriesId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}