using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    public class MVCBookContext : DbContext
    {
        public MVCBookContext(DbContextOptions<MVCBookContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
