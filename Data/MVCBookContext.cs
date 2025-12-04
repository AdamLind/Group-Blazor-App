using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    public class MVCBookContext : IdentityDbContext
    {
        public MVCBookContext(DbContextOptions<MVCBookContext> options)
        : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
