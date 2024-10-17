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

            modelBuilder.Entity<RefreshToken>().HasKey(e => e.Jti);
            modelBuilder.Entity<UserAccToken>().HasKey(e => e.Token);
        }
    }
}
