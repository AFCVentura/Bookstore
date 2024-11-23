using Bookstore.Data;
using Bookstore.Models;

namespace Bookstore.Services.Seeding
{
    public class SeedingService
    {
        private readonly BookstoreContext _context;
        public SeedingService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            if (_context.Genres.Any() ||
               _context.Books.Any() ||
               _context.Sales.Any() ||
               _context.Sellers.Any()
               )
            {
                return; // Db já foi semeado.
            }

            // Gêneros
            Genre g1 = new Genre(1, "Literatura Americana");
            Genre g2 = new Genre(2, "Romance de Aventura");
            Genre g3 = new Genre(3, "História do Mar");
            Genre g4 = new Genre(4, "Ficção Científica");
            Genre g5 = new Genre(5, "Fantasia");
            Genre g6 = new Genre(6, "Ficção Histórica");
            Genre g7 = new Genre(7, "Biografia");
            Genre g8 = new Genre(8, "Terror");
            Genre g9 = new Genre(9, "Mistério");
            Genre g10 = new Genre(10, "Literatura Brasileira");
            Genre g11 = new Genre(11, "Distopia");
            Genre g12 = new Genre(12, "Ficção Política");
            Genre g13 = new Genre(13, "Literatura Britânica");
            Genre g14 = new Genre(14, "Romance Policial");
            Genre g15 = new Genre(15, "Realismo Moderno");
            Genre g16 = new Genre(16, "Horror Gótico");
            Genre g17 = new Genre(17, "Literatura Russa");
            Genre g18 = new Genre(18, "Literatura Alemã");
            Genre g19 = new Genre(19, "Literatura Latino-Americana");
            Genre g20 = new Genre(20, "Poema Épico");
            Genre g21 = new Genre(21, "Tragédia");

            // Livros
            Book b1 = new Book(1, "Moby Dick", 89.90, "Herman Melville", 1851);
            b1.Genres.Add(g1);
            b1.Genres.Add(g2);
            b1.Genres.Add(g3);

            Book b2 = new Book(2, "1984", 74.50, "George Orwell", 1949);
            b2.Genres.Add(g11);
            b2.Genres.Add(g12);
            b2.Genres.Add(g13);

            Book b3 = new Book(3, "Senhor dos Anéis: A Sociedade do Anel", 120.00, "J.R.R. Tolkien", 1954);
            b3.Genres.Add(g5);
            b3.Genres.Add(g2);

            Book b4 = new Book(4, "O Código Da Vinci", 67.90, "Dan Brown", 2003);
            b4.Genres.Add(g9);
            b4.Genres.Add(g14);

            Book b5 = new Book(5, "Dom Quixote", 92.70, "Miguel de Cervantes", 1605);
            b5.Genres.Add(g2);

            Book b6 = new Book(6, "A Guerra dos Tronos", 95.80, "George R.R. Martin", 1996);
            b6.Genres.Add(g5);

            Book b7 = new Book(7, "O Egípcio", 58.70, "Mika Waltari", 1945);
            b7.Genres.Add(g6);

            Book b8 = new Book(8, "O Conde de Monte Cristo", 79.90, "Alexandre Dumas", 1844);
            b8.Genres.Add(g6);
            b8.Genres.Add(g2);

            Book b9 = new Book(9, "Memórias Póstumas de Brás Cubas", 49.90, "Machado de Assis", 1881);
            b9.Genres.Add(g10);
            b9.Genres.Add(g15);

            Book b10 = new Book(10, "Frankenstein", 54.90, "Mary Shelley", 1818);
            b10.Genres.Add(g8);
            b10.Genres.Add(g4);
            b10.Genres.Add(g16);


            Book b11 = new Book(11, "Drácula", 69.99, "Bram Stoker", 1897);
            b11.Genres.Add(g8);
            b11.Genres.Add(g9);
            b11.Genres.Add(g16);

            Book b12 = new Book(12, "O Hobbit", 78.99, "J.R.R. Tolkien", 1937);
            b12.Genres.Add(g5);
            b12.Genres.Add(g2);

            Book b13 = new Book(13, "A Revolução dos Bichos", 40.00, "George Orwell", 1945);
            b13.Genres.Add(g13);
            b13.Genres.Add(g12);

            Book b14 = new Book(14, "Crime e Castigo", 90.00, "Fiódor Dostoiévski", 1866);
            b14.Genres.Add(g9);
            b14.Genres.Add(g17);

            Book b15 = new Book(15, "O Senhor das Moscas", 60.00, "William Golding", 1954);
            b15.Genres.Add(g13);

            Book b16 = new Book(16, "O Nome da Rosa", 68.00, "Umberto Eco", 1980);
            b16.Genres.Add(g9);
            b16.Genres.Add(g6);
            b16.Genres.Add(g14);

            Book b17 = new Book(17, "A Metamorfose", 34.00, "Franz Kafka", 1915);
            b17.Genres.Add(g18);

            Book b18 = new Book(18, "Cem Anos de Solidão", 89.90, "Gabriel García Márquez", 1967);
            b18.Genres.Add(g19);

            Book b19 = new Book(19, "O Silmarillion", 80.00, "J.R.R. Tolkien", 1977);
            b19.Genres.Add(g5);

            Book b20 = new Book(20, "A Ilha do Tesouro", 56.00, "Robert Louis Stevenson", 1883);
            b20.Genres.Add(g2);
            b20.Genres.Add(g3);
            b20.Genres.Add(g13);

            Book b21 = new Book(21, "Guerra e Paz", 110.00, "Liev Tolstói", 1869);
            b21.Genres.Add(g6);

            Book b22 = new Book(22, "1984", 75.00, "George Orwell", 1949);
            b22.Genres.Add(g11);
            b22.Genres.Add(g12);
            b22.Genres.Add(g13);

            Book b23 = new Book(23, "A Divina Comédia", 130.00, "Dante Alighieri", 1320);
            b23.Genres.Add(g6);
            b23.Genres.Add(g20);

            Book b24 = new Book(24, "Macbeth", 49.90, "William Shakespeare", 1606);
            b24.Genres.Add(g13);
            b24.Genres.Add(g21);

            Book b25 = new Book(25, "As Aventuras de Sherlock Holmes", 60.00, "Arthur Conan Doyle", 1892);
            b25.Genres.Add(g9);
            b25.Genres.Add(g14);
            b25.Genres.Add(g13);

            // Funcionários
            Seller se1 = new Seller(1, "Gabriel Barbosa", "gabriel.pescoco@gmail.com", 4000);
            Seller se2 = new Seller(2, "Mariana Sousa", "mariana.sousa@gmail.com", 3500);
            Seller se3 = new Seller(3, "João Ferreira", "joao.ferreira@gmail.com", 3700);
            Seller se4 = new Seller(4, "Paula Castro", "paula.castro@gmail.com", 4100);
            Seller se5 = new Seller(5, "Luiz Oliveira", "luiz.oliveira@gmail.com", 3800);

            // Vendas
            Sale sa1 = new Sale(1);
            sa1.Books.Add(b1);
            b1.Sales.Add(sa1);
            se1.Sales.Add(sa1);
            sa1.Seller = se1;

            Sale sa2 = new Sale(2);
            sa2.Books.Add(b2);
            b2.Sales.Add(sa2);
            se2.Sales.Add(sa2);
            sa2.Seller = se2;

            // Criar mais 48 vendas
            Sale sa3 = new Sale(3);
            sa3.Books.Add(b3);
            sa3.Books.Add(b4);
            b3.Sales.Add(sa3);
            b4.Sales.Add(sa3);
            se3.Sales.Add(sa3);
            sa3.Seller = se3;

            Sale sa4 = new Sale(4);
            sa4.Books.Add(b5);
            b5.Sales.Add(sa4);
            se4.Sales.Add(sa4);
            sa4.Seller = se4;

            Sale sa5 = new Sale(5);
            sa5.Books.Add(b6);
            sa5.Books.Add(b7);
            b6.Sales.Add(sa5);
            b7.Sales.Add(sa5);
            se5.Sales.Add(sa5);
            sa5.Seller = se5;

            Sale sa6 = new Sale(6);
            sa6.Books.Add(b6);
            sa6.Books.Add(b7);
            b6.Sales.Add(sa6);
            b7.Sales.Add(sa6);
            se1.Sales.Add(sa6);
            sa6.Seller = se1;

            Sale sa7 = new Sale(7);
            sa7.Books.Add(b8);
            b8.Sales.Add(sa7);
            se2.Sales.Add(sa7);
            sa7.Seller = se2;

            Sale sa8 = new Sale(8);
            sa8.Books.Add(b9);
            b9.Sales.Add(sa8);
            se3.Sales.Add(sa8);
            sa8.Seller = se3;

            Sale sa9 = new Sale(9);
            sa9.Books.Add(b10);
            b10.Sales.Add(sa9);
            se4.Sales.Add(sa9);
            sa9.Seller = se4;

            Sale sa10 = new Sale(10);
            sa10.Books.Add(b11);
            b11.Sales.Add(sa10);
            se5.Sales.Add(sa10);
            sa10.Seller = se5;

            Sale sa11 = new Sale(11);
            sa11.Books.Add(b12);
            b12.Sales.Add(sa11);
            se1.Sales.Add(sa11);
            sa11.Seller = se1;

            Sale sa12 = new Sale(12);
            sa12.Books.Add(b13);
            b13.Sales.Add(sa12);
            se2.Sales.Add(sa12);
            sa12.Seller = se2;

            Sale sa13 = new Sale(13);
            sa13.Books.Add(b14);
            b14.Sales.Add(sa13);
            se3.Sales.Add(sa13);
            sa13.Seller = se3;

            Sale sa14 = new Sale(14);
            sa14.Books.Add(b15);
            b15.Sales.Add(sa14);
            se4.Sales.Add(sa14);
            sa14.Seller = se4;

            Sale sa15 = new Sale(15);
            sa15.Books.Add(b16);
            b16.Sales.Add(sa15);
            se5.Sales.Add(sa15);
            sa15.Seller = se5;

            Sale sa16 = new Sale(16);
            sa16.Books.Add(b17);
            b17.Sales.Add(sa16);
            se1.Sales.Add(sa16);
            sa16.Seller = se1;

            Sale sa17 = new Sale(17);
            sa17.Books.Add(b18);
            b18.Sales.Add(sa17);
            se2.Sales.Add(sa17);
            sa17.Seller = se2;

            Sale sa18 = new Sale(18);
            sa18.Books.Add(b19);
            b19.Sales.Add(sa18);
            se3.Sales.Add(sa18);
            sa18.Seller = se3;

            Sale sa19 = new Sale(19);
            sa19.Books.Add(b20);
            b20.Sales.Add(sa19);
            se4.Sales.Add(sa19);
            sa19.Seller = se4;

            Sale sa20 = new Sale(20);
            sa20.Books.Add(b21);
            b21.Sales.Add(sa20);
            se5.Sales.Add(sa20);
            sa20.Seller = se5;

            Sale sa21 = new Sale(21);
            sa21.Books.Add(b22);
            b22.Sales.Add(sa21);
            se1.Sales.Add(sa21);
            sa21.Seller = se1;

            Sale sa22 = new Sale(22);
            sa22.Books.Add(b23);
            b23.Sales.Add(sa22);
            se2.Sales.Add(sa22);
            sa22.Seller = se2;

            Sale sa23 = new Sale(23);
            sa23.Books.Add(b24);
            b24.Sales.Add(sa23);
            se3.Sales.Add(sa23);
            sa23.Seller = se3;

            Sale sa24 = new Sale(24);
            sa24.Books.Add(b25);
            b25.Sales.Add(sa24);
            se4.Sales.Add(sa24);
            sa24.Seller = se4;

            Sale sa25 = new Sale(25);
            sa25.Books.Add(b1);
            sa25.Books.Add(b2);
            b1.Sales.Add(sa25);
            b2.Sales.Add(sa25);
            se5.Sales.Add(sa25);
            sa25.Seller = se5;

            Sale sa26 = new Sale(26);
            sa26.Books.Add(b3);
            sa26.Books.Add(b4);
            b3.Sales.Add(sa26);
            b4.Sales.Add(sa26);
            se1.Sales.Add(sa26);
            sa26.Seller = se1;

            Sale sa27 = new Sale(27);
            sa27.Books.Add(b5);
            b5.Sales.Add(sa27);
            se2.Sales.Add(sa27);
            sa27.Seller = se2;

            Sale sa28 = new Sale(28);
            sa28.Books.Add(b6);
            b6.Sales.Add(sa28);
            se3.Sales.Add(sa28);
            sa28.Seller = se3;

            Sale sa29 = new Sale(29);
            sa29.Books.Add(b7);
            b7.Sales.Add(sa29);
            se4.Sales.Add(sa29);
            sa29.Seller = se4;

            Sale sa30 = new Sale(30);
            sa30.Books.Add(b8);
            sa30.Books.Add(b9);
            b8.Sales.Add(sa30);
            b9.Sales.Add(sa30);
            se5.Sales.Add(sa30);
            sa30.Seller = se5;

            Sale sa31 = new Sale(31);
            sa31.Books.Add(b10);
            b10.Sales.Add(sa31);
            se1.Sales.Add(sa31);
            sa31.Seller = se1;

            Sale sa32 = new Sale(32);
            sa32.Books.Add(b11);
            b11.Sales.Add(sa32);
            se2.Sales.Add(sa32);
            sa32.Seller = se2;

            Sale sa33 = new Sale(33);
            sa33.Books.Add(b12);
            b12.Sales.Add(sa33);
            se3.Sales.Add(sa33);
            sa33.Seller = se3;

            Sale sa34 = new Sale(34);
            sa34.Books.Add(b13);
            b13.Sales.Add(sa34);
            se4.Sales.Add(sa34);
            sa34.Seller = se4;

            Sale sa35 = new Sale(35);
            sa35.Books.Add(b14);
            b14.Sales.Add(sa35);
            se5.Sales.Add(sa35);
            sa35.Seller = se5;

            Sale sa36 = new Sale(36);
            sa36.Books.Add(b15);
            b15.Sales.Add(sa36);
            se1.Sales.Add(sa36);
            sa36.Seller = se1;

            Sale sa37 = new Sale(37);
            sa37.Books.Add(b16);
            b16.Sales.Add(sa37);
            se2.Sales.Add(sa37);
            sa37.Seller = se2;

            Sale sa38 = new Sale(38);
            sa38.Books.Add(b17);
            b17.Sales.Add(sa38);
            se3.Sales.Add(sa38);
            sa38.Seller = se3;

            Sale sa39 = new Sale(39);
            sa39.Books.Add(b18);
            b18.Sales.Add(sa39);
            se4.Sales.Add(sa39);
            sa39.Seller = se4;

            Sale sa40 = new Sale(40);
            sa40.Books.Add(b19);
            b19.Sales.Add(sa40);
            se5.Sales.Add(sa40);
            sa40.Seller = se5;

            Sale sa41 = new Sale(41);
            sa41.Books.Add(b20);
            b20.Sales.Add(sa41);
            se1.Sales.Add(sa41);
            sa41.Seller = se1;

            Sale sa42 = new Sale(42);
            sa42.Books.Add(b21);
            b21.Sales.Add(sa42);
            se2.Sales.Add(sa42);
            sa42.Seller = se2;

            Sale sa43 = new Sale(43);
            sa43.Books.Add(b22);
            b22.Sales.Add(sa43);
            se3.Sales.Add(sa43);
            sa43.Seller = se3;

            Sale sa44 = new Sale(44);
            sa44.Books.Add(b23);
            b23.Sales.Add(sa44);
            se4.Sales.Add(sa44);
            sa44.Seller = se4;

            Sale sa45 = new Sale(45);
            sa45.Books.Add(b24);
            b24.Sales.Add(sa45);
            se5.Sales.Add(sa45);
            sa45.Seller = se5;

            Sale sa46 = new Sale(46);
            sa46.Books.Add(b25);
            b25.Sales.Add(sa46);
            se1.Sales.Add(sa46);
            sa46.Seller = se1;

            Sale sa47 = new Sale(47);
            sa47.Books.Add(b1);
            b1.Sales.Add(sa47);
            se2.Sales.Add(sa47);
            sa47.Seller = se2;

            Sale sa48 = new Sale(48);
            sa48.Books.Add(b2);
            b2.Sales.Add(sa48);
            se3.Sales.Add(sa48);
            sa48.Seller = se3;

            Sale sa49 = new Sale(49);
            sa49.Books.Add(b3);
            b3.Sales.Add(sa49);
            se4.Sales.Add(sa49);
            sa49.Seller = se4;

            Sale sa50 = new Sale(50);
            sa50.Books.Add(b4);
            b4.Sales.Add(sa50);
            se5.Sales.Add(sa50);
            sa50.Seller = se5;


            // Adicionando Gêneros
            await _context.Genres.AddRangeAsync(
                g1, g2, g3, g4, g5, g6, g7, g8, g9, g10, g11, g12, g13, g14, g15, g16, g17, g18, g19, g20, g21
            );

            // Adicionando Livros
            await _context.Books.AddRangeAsync(
                b1, b2, b3, b4, b5, b6, b7, b8, b9, b10,
                b11, b12, b13, b14, b15, b16, b17, b18, b19, b20,
                b21, b22, b23, b24, b25
            );

            // Adicionando Vendedores
            await _context.Sellers.AddRangeAsync(
                se1, se2, se3, se4, se5
            );

            // Adicionando Vendas
            await _context.Sales.AddRangeAsync(
                sa1, sa2, sa3, sa4, sa5, sa6, sa7, sa8, sa9, sa10,
                sa11, sa12, sa13, sa14, sa15, sa16, sa17, sa18, sa19, sa20,
                sa21, sa22, sa23, sa24, sa25, sa26, sa27, sa28, sa29, sa30,
                sa31, sa32, sa33, sa34, sa35, sa36, sa37, sa38, sa39, sa40,
                sa41, sa42, sa43, sa44, sa45, sa46, sa47, sa48, sa49, sa50
            );

            // Salvando alterações no banco
            await _context.SaveChangesAsync();
        }
    }
}
