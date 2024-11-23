using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Title { get; set; }

        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public double Price { get; set; }

        [Display(Name = "Autor")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Author { get; set; }

        [Display(Name = "Ano de lançamento")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int ReleaseYear { get; set; }

        [Display(Name = "Vendas")]
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();

        [Display(Name = "Gêneros Literários")]
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();


        private static readonly int _currentYear = DateTime.Now.Year;

        public Book()
        {

        }
        public Book(int id, string title, double price, string author, int releaseYear)
        {
            Id = id;
            Title = title;
            Price = price;
            Author = author;
            ReleaseYear = releaseYear;
        }



    }
}
