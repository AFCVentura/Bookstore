using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.ViewModels
{
    public class BookFormViewModel
    {
        // No Create, esse é o livro que será criado
        // No Edit, é o livro que está sendo editado
        public Book Book { get; set; }

        // Essa é a lista de gêneros disponíveis, buscados no banco de dados
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        // Essa é a lista de gêneros que foram selecionados
        // Usaremos essa propriedade para verificar se foi selecionado algum gênero
        // E também para armazenar quais gêneros o usuário selecionou
        [Display(Name = "Gêneros Literários")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public List<int> SelectedGenresIds { get; set; } = new List<int>();

        // Embora tenha uma estrutura diferente, isso é uma propriedade também, mas apenas de leitura
        // Isso porque nós já definimos o valor dela de cara
        // O valor dela é o que o método ao lado da arrow function retornar
        public List<SelectListItem> GenresSelect => GenerateGenresSelect();

        // Para exibir um item naquelas listas de seleção, ele precisa ser um objeto do tipo SelectListItem
        // Aqui pegamos a lista de gêneros, percorremos ela e criamos um objeto desse tipo para cada um
        // Cada um deles tem dois atributos, o Value e o Text
        // O Text é o que aparece na tela, queremos que apareça o nome do gênero correspondente
        // O Value é o que é salvo numa lista quando o usuário seleciona uma opção
        // Queremos que sejam salvos os ids dos gêneros que forem selecionados
        private List<SelectListItem> GenerateGenresSelect()
        {
            List<SelectListItem> genresSelect = new List<SelectListItem>();
            if (Genres is not null)
            {
                foreach (Genre genre in Genres)
                {
                    genresSelect.Add(new SelectListItem { Value = genre.Id.ToString(), Text = genre.Name });
                }
            }
            return genresSelect;
        }
    }
}
