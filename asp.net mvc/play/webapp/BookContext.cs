using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using webapp.Models;

namespace webapp
{
    public class BookContext : DbContext
    {
        public BookContext() : base("BookContext")
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Author { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}