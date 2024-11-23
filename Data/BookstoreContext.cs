using Bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    // Nosso Context herda da classe DbContext, você vai precisar instalar o pacote Microsoft.EntityFrameworkCore
    public class BookstoreContext : DbContext
    {
        // Esse é o construtor do nosso Context, o único parâmetro passado são as opções, que a gente delega pro construtor base instanciar.
        public BookstoreContext(DbContextOptions<BookstoreContext> options) : base(options)
        {
        }

        // Criamos uma propriedade DbSet de cada model.
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Seller> Sellers { get; set; }
    }
}
