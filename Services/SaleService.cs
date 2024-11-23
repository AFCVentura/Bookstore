
using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class SaleService
    {
        private readonly BookstoreContext _context;
        public SaleService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> FindAllAsync()
        {
            return await _context.Sales.Include(x => x.Seller).Include(x => x.Books).ToListAsync();
        }

        public async Task<Sale> FindByIdEagerAsync(int id)
        {
            return await _context.Sales.Include(x => x.Seller).Include(x => x.Books).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Sale> FindByIdAsync(int id)
        {
            return await _context.Sales.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task InsertAsync(Sale sale)
        {
            _context.Add(sale);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Sales.FindAsync(id);
                _context.Sales.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new IntegrityException(ex.Message);
            }
        }

        public async Task UpdateAsync(SaleFormViewModel viewModel)
        {
            bool hasAny = await _context.Sales.AnyAsync(x => x.Id == viewModel.Sale.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }

            try
            {
                Sale? dbSale = await _context.Sales.Include(x => x.Books).Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == viewModel.Sale.Id);

                List<Book> selectedBooks = new List<Book>();

                foreach (int bookId in viewModel.SelectedBooksIds)
                {
                    Book book = _context.Books.FirstOrDefault(x => x.Id == bookId);

                    if (book is not null)
                    {
                        selectedBooks.Add(book);
                    }
                }

                List<Book> currentBooks = dbSale.Books.ToList();

                List<Book> booksToRemove = currentBooks.Where(current => !selectedBooks.Any(selected => selected.Id == current.Id)).ToList();

                List<Book> booksToAdd = selectedBooks.Where(selected => !currentBooks.Any(current => current.Id == selected.Id)).ToList();

                foreach (Book book in booksToRemove)
                {
                    dbSale.Books.Remove(book);
                }

                foreach (Book book in booksToAdd)
                {
                    dbSale.Books.Add(book);
                }


                Seller seller = await _context.Sellers.FirstOrDefaultAsync(x => x.Id == viewModel.SelectedSellerId);
                dbSale.Seller = seller;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcorrencyException(ex.Message);
            }
        }
    }
}
