# Finalizando Models

## Tela de detalhes do Genre

Agora queremos criar a tela que exibe os detalhes do gênero, mas não vai ter graça se ela só tiver o nome do gênero, né? Então queremos na verdade exibir todos os livros que sejam daquele gênero. Para fazer isso, precisamos implementar um conceito importante chamado Eager Loading, mas antes de explicar o que é isso e implementarmos, precisaremos primeiro ajustar os Models incompletos que faltaram, então vamos fazer eles e uma nova Migration!

#### Diagrama para relembrar:

![Diagrama de classe do projeto](./10restoDoCRUDGenre/diagramaClasseProjeto.png)

### Genre

Vamos remover o comentário daquela linha de código da coleção de Books:

```c#
// Muitos livros
[Display(Name = "Livros")]
public ICollection<Book> Books { get; set; } = new List<Book>();
```

### Book

```c#
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
    public Book(int id, string title, string author, int releaseYear)
    {
        Id = id;
        Title = title;
        Author = author;
        ReleaseYear = releaseYear;
    }
}
```

Note as anotações que a gente usou para definir coisas como a formatação padrão do dinheiro.

### Sale

```c#
public class Sale
{
    public int Id { get; set; }

    [Display(Name = "Data")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime Date { get; set; } = DateTime.Now;

    [Display(Name = "Valor")]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double Amount => CalculateTotalAmount();

    [Display(Name = "Vendedor")]
    public Seller Seller { get; set; }

    [Display(Name = "Livros")]
    public ICollection<Book> Books { get; set; } = new List<Book>();


    public Sale()
    {

    }
    public Sale(int id)
    {
        Id = id;
    }

    private double CalculateTotalAmount()
    {
        return Books.Sum(book => book.Price);
    }
}
```

Note aqui também a formatação de datas e o uso do LINQ para buscar o preço de todos os livros, essa classe também tem uma propriedade derivada (representada por uma / no diagrama), essa propriedade não pode ter seu valor editado, já que o valor é definido a ela através de uma expressão, no caso, através do método `CalculateTotalAmount()`, por isso ele é privado, ele é usado apenas para definir o valor dessa propriedade, logo não precisa e nem deve estar disponível para outras classes usarem.

### Seller

```c#
public class Seller
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [Display(Name = "Nome")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Insira um email válido")]
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Email { get; set; }

    [Display(Name = "Salário")]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public double Salary { get; set; }
    
    [Display(Name = "Vendas")]
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public Seller() { }

    public Seller(int id, string name, string email, double salary)
    {
        Id = id;
        Name = name;
        Email = email;
        Salary = salary;
    }

    public double CalculateTotalSalesAmount()
    {
        return Sales.Sum(sale => sale.Amount);
    }

}
```

Note aqui que o Asp.Net possui até uma anotação para email, onde ele verifica sozinho se o email está no formato apropriado.

## Fazendo Migration

Depois de editar todos os Models certinho, a gente pode fazer a Migration da mesma forma que fizemos da primeira vez, mas mudando o nome de "Initial" para outra coisa, eu vou chamar de "Other-Entities".

No **Package Manager Console**:

```shell
Add-Migration Other-Entities
```

```shell
Update-Database
```

Ou no **Powershell**:

```shell
dotnet tool install --global dotnet-ef
```

```shell
dotnet ef migrations add Other-Entities
```

```shell
dotnet ef database update
```

### Adicionando novos DbSet<>

Para conseguir manipular as tabelas do banco de dados usando o EF Core, precisamos adicionar no nosso DbContext, os DbSet de cada tabela.
