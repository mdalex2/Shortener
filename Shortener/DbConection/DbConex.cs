using Microsoft.EntityFrameworkCore;
using Shortener.Models;

namespace Shortener.DbConection
{
    public class DbConex : DbContext    
    {
        public DbConex(DbContextOptions<DbConex> options) : base(options)
        {

        }
        public DbSet<UrlConfigs> UrlConfigs { get; set; }
    }
}
