using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.ViewModels
{
    public class SaleFormViewModel
    {
        public Sale Sale { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<Seller> Sellers { get; set; } = new List<Seller>();

        [Display(Name = "Livros")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public ICollection<int> SelectedBooksIds { get; set; } = new List<int>();

        [Display(Name = "Vendedor")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int SelectedSellerId { get; set; }

        public List<SelectListItem> BooksSelect => GenerateBooksSelect();

        public List<SelectListItem> SellersSelect => GenerateSellersSelect();


        private List<SelectListItem> GenerateBooksSelect()
        {
            List<SelectListItem> booksSelect = new List<SelectListItem>();
            if (Books is not null)
            {
                foreach (Book book in Books)
                {
                    booksSelect.Add(new SelectListItem { Value = book.Id.ToString(), Text = book.Title });
                }
            }
            return booksSelect;
        }

        private List<SelectListItem> GenerateSellersSelect()
        {
            List<SelectListItem> sellersSelect = new List<SelectListItem>();
            if (Books is not null)
            {
                foreach (Seller seller in Sellers)
                {
                    sellersSelect.Add(new SelectListItem { Value = seller.Id.ToString(), Text = seller.Name });
                }
            }
            return sellersSelect;
        }

    }
}
