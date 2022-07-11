using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Restaurants
{
    public class RestaurantContext : DbContext
    {
        private readonly string connectionString;

        public RestaurantContext(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value should not be empty.", nameof(connectionString));

            this.connectionString = connectionString;
        }

        public DbSet<Restaurant> Restaurants { get; set; }

        public bool IsNew<TEntity>(TEntity entity) where TEntity : class
        {
            return !this.Set<TEntity>().Local.Any(e => e == entity);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Change this to 'UseSqlServer' if you use sqlserver of course you need the right nuget package
            optionsBuilder.UseSqlite(this.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .HasKey(restaurant => restaurant.RestaurantId);
        }
    }
}