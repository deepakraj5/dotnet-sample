using Microsoft.EntityFrameworkCore;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserAccToken> UserAccTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.HasKey(e => e.Jti);
                e.Property(e => e.ExpiresIn).HasConversion(
                    v => v.UtcDateTime,
                    v => new DateTimeOffset(v));
            });
            modelBuilder.Entity<UserAccToken>().HasKey(e => e.Token);
        }
    }
}
