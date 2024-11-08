# Implementando Sales

Agora chegou a etapa de adicionar a entidade de vendas, para acelerar mais o processo, vamos fazer uso do **Crud Scaffolding** novamente. Além disso, já vamos refatorar tanto a utilização de Services quanto o Eager Loading e as ViewModels de uma vez só.

A entidade Sale vai ser a maior, isso porque precisamos permitir na edição e criação, que seja manipulado tanto o vendedor que a fez quanto os livros dela, o que envolve três entidades, no Genre e no Seller não precisamos adicionar nada além da própria entidade na criação e edição dos dados, já no Book precisamos fazer o processo com os Genres dele, agora precisaremos fazer com os Books da venda e o Seller também.

Vamos começar fazendo a Action Index do Controller:

## Index do Controller

```c#
// GET Sales
public async Task<IActionResult> Index()
{
    List<Sale> sales = await _service.FindAllAsync();
    return View(sales);
}
```

Nada de novo, mas depois vamos adicionar uma funcionalidade bem legal aqui.

## FindAllAsync do Service

```c#
public async Task<List<Sale>> FindAllAsync()
{
    return await _context.Sales.Include(x => x.Seller).Include(x => x.Books).ToListAsync();
}
```
Note que usamos o Include e depois outro include, entendam a diferença entre usar dois Includes e um Include com um ThenInclude:

```
Dois Includes:
Sale -> Seller
e
Sale -> Books

Include e ThenInclude:
Sale -> Seller -> Books
```

Não faria sentido usar o ThenInclude aqui, no caso do Seller fazia:

```
Seller -> Sales -> Books
```

## View Index

```html
@model IEnumerable<Bookstore.Models.Sale>

@{
    ViewData["Title"] = "Vendas";
}

<h1 class ="text-primary-emphasis">@ViewData["Title"]</h1>

<p>
    <a asp-action="Create" class="text-primary">Adicionar nova Venda</a>
</p>
<table class="table table-hover">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Date)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Amount)
            </th>
            <th class="text-primary-emphasis">
                @Html.DisplayNameFor(model => model.Books)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Seller)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td class="text-primary-emphasis">
                @Html.DisplayFor(modelItem => item.Date)
            </td>
            <td class="text-primary-emphasis">
                @Html.DisplayFor(modelItem => item.Amount)
            </td>
                <td class="text-primary-emphasis">
                    @foreach (Book book in item.Books)
                    {
                        <a asp-controller="Books" asp-action="Details" asp-route-id="@book.Id" class="text-primary">@book.Title</a>
                        <br />
                    }
                </td>
            <td class="text-primary-emphasis">
                <a asp-controller="Sellers" asp-action="Details" asp-route-id="@item.Seller.Id" class="text-primary">@item.Seller.Name</a>
                <br />
            </td>
            <td >
                    <a type="button"
                       title="Detalhes"
                       class="btn btn-outline-info px-2 py-1 rounded mx-1"
                       asp-action="Details"
                       asp-route-id="@item.Id">
                        <i class="bi bi-list-ul"></i>
                    </a>
                    <a type="button"
                       title="Editar"
                       class="btn btn-outline-warning px-2 py-1 rounded mx-1" asp-action="Edit"
                       asp-route-id="@item.Id">
                        <i class="bi bi-pencil-fill"></i>
                    </a>
                    <a type="button"
                       title="Excluir"
                       class="btn btn-outline-danger px-2 py-1 rounded mx-1"
                       asp-action="Delete"
                       asp-route-id="@item.Id">
                        <i class="bi bi-trash-fill"></i>
                    </a>
            </td>
        </tr>
}
    </tbody>
</table>

```

## Implementando diferentes formas de ordenar as vendas

Para melhorar a experiência do usuário e também pensando no fato de que as vendas serão a entidade com mais registros, vamos implementar uma ferramenta que permite que o usuário ordene as vendas por valor, nome do vendedor ou data.

### View Index

Para fazer isso, precisamos colocar um link em cada coluna do cabeçalho da tabela correspondente a esses itens:

```html
Coluna de Data do Cabeçalho Antes:
<th>
    @Html.DisplayNameFor(model => model.Date)
</th>

E Depois:
<th>
    <a 
        asp-action="Index" 
        asp-route-sortOrder="@ViewData["DateSortParam"]"
        class="text-primary-emphasis"
    >
        @Html.DisplayNameFor(model => model.Date)
    </a>
</th>
```

Fizemos com que o cabeçalho se tornasse um link que carrega a mesma Action (Index), mas agora vamos fazer uso do atributo asp-route, que é customizável (podemos escrever asp-route-qualquercoisa) para trafegar a forma de ordenar que adotaremos, esse atributo adiciona parâmetros na URL automaticamente, podemos pegar ele nos parâmetros da Action Index e inverter a ordem, reenviando a nova forma de ordenar através do ViewData["DateSortParam"], vou detalhar isso mais pra frente.

Voltando para o controller...

### Index do Controller

```c#
// Adicionamos o parâmetro sortOrder
public async Task<IActionResult> Index(string sortOrder)
{
    List<Sale> sales = await _service.FindAllAsync();

    // Configuramos que se a forma de ordenar chega sendo date_asc (por data de forma crescente), vira date_desc (decrescente) e vice versa.
    ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

    // Aqui pegamos o sortOrder e verificamos se é por data crescente, se for, fazemos a reordenação na própria lista de sales que já enviaríamos para a view.
    switch(sortOrder) {
         case "date_desc":
            sales = sales.OrderByDescending(x => x.Date).ToList();
            break;
        case "date_asc":
            sales = sales.OrderBy(x => x.Date).ToList();
            break;
    }
```

Agora basta fazer o mesmo para o nome do vendedor e para o valor da venda

### Restante

O resultado final do Index do Controller será assim:

```c#
public async Task<IActionResult> Index(string sortOrder)
{
    List<Sale> sales = await _service.FindAllAsync();

    ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
    ViewData["AmountSortParam"] = sortOrder == "amount_asc" ? "amount_desc" : "amount_asc";
    ViewData["SellerSortParam"] = sortOrder == "seller_asc" ? "seller_desc" : "seller_asc";


    switch (sortOrder)
    {
        case "date_desc":
            sales = sales.OrderByDescending(x => x.Date).ToList();
            break;
        case "date_asc":
            sales = sales.OrderBy(x => x.Date).ToList();
            break;
        case "amount_desc":
            sales = sales.OrderByDescending(x => x.Amount).ToList();
            break;
        case "amount_asc":
            sales = sales.OrderBy(x => x.Amount).ToList();
            break;
        case "seller_desc":
            sales = sales.OrderByDescending(x => x.Seller.Name).ToList();
            break;
        case "seller_asc":
            sales = sales.OrderBy(x => x.Seller.Name).ToList();
            break;
        default:
            // O padrão é mostrar as mais novas primeiro
            sales = sales.OrderByDescending(x => x.Date).ToList();
            break;
    }

    return View(sales);
}
```

Já dos cabeçalhos será assim:

```html
<table class="table table-hover">
    <thead>
        <tr>
            <th>
                <a asp-action="Index" asp-route-sortOrder="@ViewData["DateSortParam"]" class="text-primary-emphasis">@Html.DisplayNameFor(model => model.Date)</a>
            </th>
            <th>
                <a asp-action="Index" asp-route-sortOrder="@ViewData["AmountSortParam"]" class="text-primary-emphasis">@Html.DisplayNameFor(model => model.Amount)</a>
            </th>
            <th class="text-primary-emphasis">
                @Html.DisplayNameFor(model => model.Books)
            </th>
            <th>
                <a asp-action="Index" asp-route-sortOrder="@ViewData["SellerSortParam"]" class="text-primary-emphasis"> @Html.DisplayNameFor(model => model.Seller)</a>
            </th>
            <th></th>
        </tr>
    </thead>
    ...
```

Pronto, agora temos como ordenar as vendas de formas diferentes decrescentes e crescentes.

## Details do Controller

```c#
 // GET Sales/Details/x
 public async Task<IActionResult> Details(int? id)
 {
     if (id is null)
     {
         return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
     }
     var obj = await _service.FindByIdEagerAsync(id.Value);
     if (obj is null)
     {
         return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
     }

     return View(obj);
 }
```

## FindByIdEagerAsync no Service

```c#
public async Task<Sale> FindByIdEagerAsync(int id)
{
    return await _context.Sales.Include(x => x.Seller).Include(x => x.Books).FirstOrDefaultAsync(x => x.Id == id);
}
```

Note que carregamos o Seller e os Books similar ao FindAllAsync, poderíamos ter nomeado o FindAllAsync de FindAllEagerAsync também, mas não teremos outro para ser o sem a palavra Eager, então deixa assim mesmo.

## View Details

```html
@model Bookstore.Models.Sale

@{
    ViewData["Title"] = $"Venda Nº {@Model.Id}";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<div>
    <h4 class="lead text-secondary-emphasis">Detalhes da Venda</h4>
    <hr />
    <dl class="row">
        <dt class = "col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Date)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Date)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Amount)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Amount)
        </dd>
        <dt class = "col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Books)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Book book in Model.Books)
            {
                <a asp-controller="Books" asp-action="Details" asp-route-id="@book.Id" class="text-primary">@book.Title</a>
                <br />
            }
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            <a asp-controller="Sellers" asp-action="Details" asp-route-id="@Model.Seller.Id" class="text-primary">@Model.Seller.Name</a>
        </dd>
    </dl>
</div>
<div>
    <a asp-action="Edit" asp-route-id="@Model?.Id" class="text-primary">Editar</a> |
    <a asp-action="Index" class="text-primary">Voltar para a lista</a>
</div>

```

Aqui sem segredo, fazemos um laço de repetição nos livros e colocamos um link tanto neles quanto no vendedor para ir para suas respectivas telas de detalhes.

Agora criaríamos a Action de Create, mas para cadastrar uma venda precisamos carregar todos os livros e também todos os vendedores, para que seja possível dizer quem fez a venda e quais foram os livros dela, para fazer isso, como vocês já devem ter aprendido, precisamos de uma viewModel.

## SaleFormViewModel

```c#
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
```

Essa viewModel é a maior que fizemos mas ela tem a lógica basicamente igual a para exibir os gêneros na hora de criar um livro, precisamos criar SelectListItems que são os campos que são selecionáveis na página, esses campos tem um texto visível, que devemos vincular aos nomes dos vendedores/títulos dos livros e também um valor oculto, que é o que é enviado para o controlador dizendo quem o usuário escolheu, no caso, enviamos o Id desses itens, não o nome.


## Create GET do Controller

```c#
// GET Sales/Create
public async Task<IActionResult> Create()
{
    List<Book> books = await _bookService.FindAllAsync();
    List<Seller> sellers = await _sellerService.FindAllAsync();


    SaleFormViewModel viewModel = new SaleFormViewModel { Books = books, Sellers = sellers };
    return View(viewModel);
}
```

Note como tivemos que buscar tanto os livros quanto os vendedores usando seus próprios services, logo, precisamos adicionar eles no começo da classe:

```c#
public class SalesController : Controller
{

    private readonly SaleService _service;
    private readonly BookService _bookService;
    private readonly SellerService _sellerService;

    public SalesController(SaleService service, BookService bookService, SellerService sellerService)
    {
        _service = service;
        _bookService = bookService;
        _sellerService = sellerService;
    }
    ...
```


## View Create

```c#
@model Bookstore.Models.ViewModels.SaleFormViewModel

@{
    ViewData["Title"] = "Adicionar Venda";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<h4 class="lead text-secondary-emphasis">Preencha os campos abaixo</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div class="form-group">
                <label asp-for="Sale.Date" class="control-label"></label>
                <input asp-for="Sale.Date" class="form-control" value="@DateTime.Today.ToString("yyyy-MM-dd")" />
                <span asp-validation-for="Sale.Date" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="SelectedBooksIds" class="control-label"></label>
                <br />
                <span class="text-muted">Segure Ctrl para selecionar mais de um item</span>
                <select multiple="multiple" asp-for="SelectedBooksIds" asp-items="Model.BooksSelect" placeholder="Selecione os Livros" class="form-select"></select>
                <span asp-validation-for="SelectedBooksIds" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="SelectedSellerId" class="control-label"></label>
                <br />
                <select asp-for="SelectedSellerId" asp-items="Model.SellersSelect" placeholder="Selecione o Vendedor" class="form-select"></select>
                <span asp-validation-for="SelectedSellerId" class="text-danger"></span>
            </div>

            <div class="form-group mt-3 mb-5">
                <input type="submit" value="Adicionar Venda" class="btn btn-outline-primary " />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index" class="text-primary">Voltar para a lista</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

Aqui também é basicamente a mesma coisa do livro, mas com dois campos ao invés de um, no Seller, precisamos tirar o `multiple="multiple"` pois só pode-se selecionar um vendedor.

## Create POST do Controller

```c#
// POST Sales/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(SaleFormViewModel viewModel)
{
    // O Sale.Seller sempre vem nulo, precisamos permitir que ele especificamente passe pela checagem.
    if (!ModelState.IsValid && ModelState["Sale.Seller"].RawValue is not null)
    {
        List<Book> books = await _bookService.FindAllAsync();
        List<Seller> sellers = await _sellerService.FindAllAsync();
        viewModel.Books = books;
        viewModel.Sellers = sellers;
        return View(viewModel);
    }

    foreach (int id in viewModel.SelectedBooksIds)
    {
        Book book = await _bookService.FindByIdAsync(id);

        if (book is not null)
        {
            viewModel.Sale.Books.Add(book);
        }
    }

    Seller seller = await _sellerService.FindByIdAsync(viewModel.SelectedSellerId);

    if (seller is not null)
    {
        viewModel.Sale.Seller = seller;
    }
    else
    {
        List<Book> books = await _bookService.FindAllAsync();
        List<Seller> sellers = await _sellerService.FindAllAsync();
        viewModel.Books = books;
        viewModel.Sellers = sellers;
        return View(viewModel);
    }

    await _service.InsertAsync(viewModel.Sale);

    return RedirectToAction(nameof(Index));
}
```

## InsertAsync do Service

```c#
public async Task InsertAsync(Sale sale)
{
    _context.Add(sale);
    await _context.SaveChangesAsync();
}
```

## Edit GET do Controller

```c#
// GET Sales/Edit
public async Task<IActionResult> Edit(int? id)
{
    if (id is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
    }
    Sale obj = await _service.FindByIdAsync(id.Value);
    if (obj is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Venda não encontrada" });
    }

    List<Book> books = await _bookService.FindAllAsync();
    List<Seller> sellers = await _sellerService.FindAllAsync();

    SaleFormViewModel viewModel = new SaleFormViewModel { Sale = obj, Books = books, Sellers = sellers };

    return View(viewModel);
}
```

Até agora nada de novo também tirando o fato de termos que buscar os vendedores e livros.

## FindByIdAsync do Service

```c#
public async Task<Sale> FindByIdAsync(int id)
{
    return await _context.Sales.Include(x => x.Seller).FirstOrDefaultAsync(x => x.Id == id);
}
```

Percebam como até o FindById que não é Eager carrega algum dado junto (Eager Loading), isso porque não vai haver nenhuma situação em que iremos exibir uma venda sem pelo menos seu vendedor, então até no mais básico ele vai junto.

## View Edit

```html
@model Bookstore.Models.ViewModels.SaleFormViewModel

@{
    ViewData["Title"] = "Editar Venda";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<h4 class="lead text-secondary-emphasis">Altere as informações desejadas abaixo</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <input type="hidden" asp-for="Sale.Id" />
            <div class="form-group">
                <label asp-for="Sale.Date" class="control-label"></label>
                <input asp-for="Sale.Date" class="form-control" value="@DateTime.Today.ToString("yyyy-MM-dd")" />
                <span asp-validation-for="Sale.Date" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="SelectedBooksIds" class="control-label"></label>
                <br />
                <span class="text-muted">Segure Ctrl para selecionar mais de um item</span>
                <select multiple="multiple" asp-for="SelectedBooksIds" asp-items="Model.BooksSelect" placeholder="Selecione os Livros" class="form-select"></select>
                <span asp-validation-for="SelectedBooksIds" class="text-danger"></span>
            </div>

            <div class="form-group">
                <label asp-for="SelectedSellerId" class="control-label"></label>
                <br />
                <select asp-for="SelectedSellerId" asp-items="Model.SellersSelect" placeholder="Selecione o Vendedor" class="form-select"></select>
                <span asp-validation-for="SelectedSellerId" class="text-danger"></span>
            </div>
            <div class="form-group mt-3 mb-5">
                <input type="submit" value="Salvar Alterações" class="btn btn-outline-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index" class="text-primary">Voltar para a lista</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}

```

Aqui também não tem nada muuito diferente do que já fazíamos, percebam que o processo depois de um tempo começa a ser até repetitivo, tirando é claro os pormenores de cada entidade.

## Edit POST do Controller

```c#
// POST Sales/Edit
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, SaleFormViewModel viewModel)
{
    if (!ModelState.IsValid && ModelState["Sale.Seller"].RawValue is not null)
    {
        List<Book> books = await _bookService.FindAllAsync();
        List<Seller> sellers = await _sellerService.FindAllAsync();
        viewModel.Books = books;
        viewModel.Sellers = sellers;
        return View(viewModel);
    }

    if (id != viewModel.Sale.Id)
    {
        return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
    }

    try
    {
        await _service.UpdateAsync(viewModel);
        return RedirectToAction(nameof(Index));
    }
    catch (ApplicationException ex)
    {
        return RedirectToAction(nameof(Error), new { message = ex.Message });
    }
}
```

## UpdateAsync do Service

Agora faremos o maior método do projeto, ele é similar ao método de mesmo nome no Book, mas um pouco maior por temos o Seller também.

```c#
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
```

A lógica é basicamente a mesma, ver quais livros a venda tem no banco e quais foram adicionados, então ver quais se mantiveram, quais devem sair e quais devem ficar e fazer as trocas manualmente, por fim, só trocamos o vendedor, como é apenas um vendedor, a troca é simples, se tivéssemos muitos vendedores pra uma venda só, teríamos que fazer esse processo enorme do livro pra vendedores também.

## Delete GET do Controller

```c#
// GET Sales/Delete/x
public async Task<IActionResult> Delete(int? id)
{
    if (id is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
    }

    var obj = await _service.FindByIdEagerAsync(id.Value);
    if (obj is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
    }

    return View(obj);
}
```

## View Delete

```html
@model Bookstore.Models.Sale

@{
    ViewData["Title"] = "Apagar Venda";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<h3 class="lead text-secondary-emphasis mb-4">Tem certeza que deseja apagar esta venda?</h3>
<div>
    <h4>Venda</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Date)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Date)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Amount)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Amount)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Books)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Book book in Model.Books)
            {
                <a asp-controller="Books" asp-action="Details" asp-route-id="@book.Id" class="text-primary">@book.Title</a>
                <br />
            }
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Name)
        </dd>
    </dl>
    <form asp-action="Delete">
        <input type="hidden" asp-for="Id" />
        <input type="submit" value="Apagar" class="btn btn-outline-danger" /> |
        <a asp-action="Index" class="text-primary">Voltar para a lista</a>
    </form>
</div>
```

## Delete POST do Controller

```c#
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    try
    {
        await _service.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }
    catch (IntegrityException ex)
    {
        return RedirectToAction(nameof(Error), new { message = ex.Message });
    }
}
```

## RemoveAsync do Service

```c#
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
```

Aqui também não temos nada muito diferente, o Delete é bem parecido com todos os outros.


## Error do Controller

Essa parte, como dito antes, não varia:

```c#
// GET Sales/Error
public IActionResult Error(string message)
{
    var viewModel = new ErrorViewModel
    {
        Message = message,
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    };
    return View(viewModel);
}
```

Bom, em termos de CRUD base dessas entidades, Nós finalizamos! Podemos considerar a partir daqui que o projeto está pronto, mas teremos mais um pouco de material basicamente para estilizar a tela inicial que está vazia até agora, adicionaremos alguns atalhos, tipo Adicionar Venda (que, pensando numa aplicação realmente usada, seria a tela mais acessada) e também outros dados que não mostramos aqui, tipo livros mais vendidos, vendedores com mais vendas, etc.