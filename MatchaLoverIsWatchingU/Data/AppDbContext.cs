using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //只允許get，避免外部修改DbSet
        public DbSet<WatchedProducts> WatchedProducts => Set<WatchedProducts>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }


    }
}