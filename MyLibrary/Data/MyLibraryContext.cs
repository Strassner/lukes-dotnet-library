using Microsoft.EntityFrameworkCore;
using MyLibrary.Models;

namespace MyLibrary.Data {
    public class MyLibraryContext : DbContext{
        public MyLibraryContext(DbContextOptions<MyLibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}
