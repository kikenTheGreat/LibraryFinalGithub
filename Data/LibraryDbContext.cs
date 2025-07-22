using Microsoft.EntityFrameworkCore;
using Library_Final.Models;

namespace Library_Final.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<int>();
            });

            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.ISBN).IsUnique();
                entity.Property(e => e.TotalCopies).HasDefaultValue(1);
                entity.Property(e => e.AvailableCopies).HasDefaultValue(1);
            });

            // Configure BorrowRecord entity
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.BorrowRecords)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BorrowRecords)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.FineAmount).HasColumnType("decimal(10,2)");
            });

            // Configure LogEntry entity
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.LogEntries)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed default admin user
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Create default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@library.com",
                    FullName = "System Administrator",
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );
        }
    }
}
