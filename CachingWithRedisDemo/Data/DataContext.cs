using CachingWithRedisDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CachingWithRedisDemo.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            base.OnConfiguring(optionBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
