using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Services.Exceptions;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class BookService
    {
        // Atributo privado do Context
        private readonly BookstoreContext _context;
        // Construtor passando ele
        public BookService(BookstoreContext context)
        {
            _context = context;
        }

        // GET Books/Index
        public async Task<List<Book>> FindAllAsync()
        {
            return await _context.Books.Include(x => x.Genres).ToListAsync();
        }

        public async Task<Book> FindByIdAsync(int id)
        {
            return await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
        }

        // POST Books/Create
        public async Task InsertAsync(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookFormViewModel viewmodel)
        {
            bool hasAny = await _context.Books.AnyAsync(x => x.Id == viewmodel.Book.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }

            try
            {
                // Livro do banco de dados
                Book? dbBook = await _context.Books.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == viewmodel.Book.Id);

                // Lista de gêneros selecionados na View
                List<Genre> selectedGenres = new List<Genre>();

                // Para cada id na lista de ids selecionados na view
                foreach (int genreId in viewmodel.SelectedGenresIds)
                {
                    // Busca qual é o gênero desse Id
                    Genre genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                    // Se de fato tiver um gênero
                    if (genre is not null)
                    {
                        // Põe ele na lista de ids selecionados
                        selectedGenres.Add(genre);
                    }
                }
                // Lista de gêneros que o livro tem atualmente no banco
                List<Genre> currentGenres = dbBook.Genres.ToList();

                // Busca todos os gêneros atuais que NÃO estão nos selecinados (gêneros que estavam e não estão mais)
                List<Genre> genresToRemove = currentGenres.Where(current => !selectedGenres.Any(selected => selected.Id == current.Id)).ToList();

                // Lógica contrária
                // Busca todos os gêneros selecionados que NÃO estão nos atuais (gêneros que não estavam e agora estarão)
                List<Genre> genresToAdd = selectedGenres.Where(selected => !currentGenres.Any(current => current.Id == selected.Id)).ToList();

                foreach (Genre genre in genresToRemove)
                {
                    dbBook.Genres.Remove(genre);
                }

                foreach (Genre genre in genresToAdd)
                {
                    _context.Attach(dbBook);
                    dbBook.Genres.Add(genre);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcorrencyException(ex.Message);
            }
        }

        // POST Books/Delete/x
        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Books.FindAsync(id);
                _context.Books.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new IntegrityException(ex.Message);
            }
        }


    }
}
