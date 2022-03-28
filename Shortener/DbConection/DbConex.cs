using Microsoft.EntityFrameworkCore;
using Shortener.Models;

namespace Shortener.DbConection
{
    public class DbConex : DbContext    
    {
        public DbConex(DbContextOptions<DbConex> options) : base(options)
        {

        }
        public DbSet<UrlShort> UrlShorts { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UrlShort>()
                .HasIndex(u => u.UrlCorta)
                .IsUnique();

        }
    }
}
