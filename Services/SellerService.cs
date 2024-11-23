using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Services.Exceptions;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Services
{
    public class SellerService
    {
        private readonly BookstoreContext _context;

        public SellerService(BookstoreContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Sellers.Include(x => x.Sales).ThenInclude(x => x.Books).ToListAsync();
        }

        public async Task<Seller> FindByIdEagerAsync(int id)
        {
            return await _context.Sellers.Include(x => x.Sales).ThenInclude(x => x.Books).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Sellers.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task InsertAsync(Seller seller)
        {
            _context.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Sellers.FindAsync(id);
                _context.Sellers.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new IntegrityException(ex.Message);
            }
        }

        public async Task UpdateAsync(Seller seller)
        {
            bool hasAny = await _context.Sellers.AnyAsync(x => x.Id == seller.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id não encontrado");
            }

            try
            {
                _context.Update(seller);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbConcorrencyException(ex.Message);
            }
        }
    }
}
