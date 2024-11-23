
namespace Bookstore.Models.ViewModels
{
    public class SellerDetailsViewModel
    {
        public Seller Seller { get; set; }
        public ICollection<Sale> RecentSales => FoundRecentSales();
        public ICollection<Sale> BiggestSales => FoundBiggestSales();

        private ICollection<Sale> FoundRecentSales()
        {
            return Seller.Sales.OrderByDescending(x => x.Date).Take(5).ToList();
        }

        private ICollection<Sale> FoundBiggestSales()
        {
            return Seller.Sales.OrderByDescending(x => x.Amount).Take(5).ToList();
        }
    }
}
