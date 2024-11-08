# Implementando Seller

A entidade Seller vai ser uma das mais tranquilas, tendo em vista que ela é uma das "extremidades" do nosso diagrama de classe e, por exemplo, para criar um vendedor, não precisamos dizer as vendas dele nem nada do tipo, basta preencher os seus atributos normais.

## Model

Vamos começar pela Model, na teoria já deve existir, mas confiram se a de vocês está assim:

```c#
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
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
}
```

Agora vamos prosseguir para o controller e o service.

## Index do Controller

Base do Controller e Action Index:

```c#
public class SellersController : Controller
{
    private readonly SellerService _service;

    public SellersController(SellerService service)
    {
        _service = service;
    }

    // GET: Sellers
    public async Task<IActionResult> Index()
    {
        return View(await _service.FindAllAsync());
    }

    ...
```

## FindAllAsync do Service 

Método correspondente no Service:

```c#
public class SellerService
{
    private readonly BookstoreContext _context;

    public SellerService(BookstoreContext context)
    {
        _context = context;
    }

    public async Task<List<Seller>> FindAllAsync()
    {
        return await _context.Sellers.Include(x => x.Sales).    ThenInclude(x => x.Books).ToListAsync();
    }   
```
Não esqueçam de adicionar o service na lista de services no `Program.cs`:

`builder.Services.AddScoped<SellerService>();`

## View Index

Agora vamos para a tela Index:

```html
@model IEnumerable<Bookstore.Models.Seller>

@{
    ViewData["Title"] = "Vendedores";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<p>
    <a asp-action="Create" class="text-primary">Adicionar novo Vendedor</a>
</p>
<table class="table table-hover">
    <thead>
        <tr>
            <th class="text-primary-emphasis">
                @Html.DisplayNameFor(model => model.Name)
            </th>
            <th class="text-primary-emphasis">
                @Html.DisplayNameFor(model => model.Email)
            </th>
            <th class="text-primary-emphasis">
                @Html.DisplayNameFor(model => model.Salary)
            </th>
            <th class="text-primary-emphasis">
                Total de Vendas
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td class="text-primary-emphasis">
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td class="text-primary-emphasis">
                @Html.DisplayFor(modelItem => item.Email)
            </td>
            <td class="text-primary-emphasis">
                @Html.DisplayFor(modelItem => item.Salary)
            </td>
            <td class="text-primary-emphasis">
                @item.CalculateTotalSalesAmount().ToString("C2")
            </td>
            <td>
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

Note que até agora nada de novo.

## Create GET do Controller

```c#
// GET: Sellers/Create
public IActionResult Create()
{
    return View();
}
```

Código enorme né?

## View Create

```html
@model Bookstore.Models.Seller

@{
    ViewData["Title"] = "Adicionar Vendedor";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<h4 class="lead text-secondary-emphasis">Preencha os campos abaixo</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Create">
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Email" class="control-label"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Salary" class="control-label"></label>
                <div class="input-group">
                    <span class="input-group-text">R$</span>
                    <input asp-for="Salary" class="form-control" />
                </div>
                <span asp-validation-for="Salary" class="text-danger"></span>
            </div>
            <div class="form-group mt-3 mb-5">
                <input type="submit" value="Adicionar Vendedor" class="btn btn-outline-primary" />
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

Aqui também, nada de novo.

## Create POST do Controller

```c#
// POST: Sellers/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Seller seller)
{
    if (!ModelState.IsValid)
    {
        return View();
    }

    await _service.InsertAsync(seller);

    return RedirectToAction(nameof(Index));
}
```

## InsertAsync do Service

```c#
public async Task InsertAsync(Seller seller)
{
    _context.Add(seller);
    await _context.SaveChangesAsync();
}
```

## Edit GET do Controller

```c#
// GET: Sellers/Edit/5
public async Task<IActionResult> Edit(int? id)
{
    if (id is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Id não fornecido" });
    }
    var obj = await _service.FindByIdAsync(id.Value);
    if (obj is null)
    {
        return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });
    }
    return View(obj);
}
```

## FindByIdAsync do Service

```c#
public async Task<Seller> FindByIdAsync(int id)
{
    return await _context.Sellers.FirstOrDefaultAsync(x => x.Id == id);
}
```

## View Edit

```html
@using System.Globalization
@model Bookstore.Models.Seller

@{
    ViewData["Title"] = "Editar Vendedor";

    Model.Salary = double.Parse(Model.Salary.ToString(CultureInfo.InvariantCulture));
}

<h1>Edit</h1>

<h4>Seller</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Email" class="control-label"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Salary" class="control-label"></label>
                <div class="input-group">
                    <span class="input-group-text">R$</span>
                    <input asp-for="Salary" class="form-control" />
                </div>
                <span asp-validation-for="Salary" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Salvar Alterações" class="btn btn-primary" />
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

## Edit POST do Controller

```c#
// POST: Sellers/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Seller seller)
{
    if (!ModelState.IsValid)
    {
        return View();
    }

    if (id != seller.Id)
    {
        return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
    }

    try
    {
        await _service.UpdateAsync(seller);
        return RedirectToAction(nameof(Index));
    }
    catch (ApplicationException ex)
    {
        return RedirectToAction(nameof(Error), new { message = ex.Message });
    }
}
```

## UpdateAsync do Service

```c#
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
```

## ViewModel SellerDetailsViewModel

Agora chegamos na hora de exibir mais informações sobre o vendedor, dentre as informações que devemos exibir, estão as vendas, mas para não exibir simplesmente todas as vendas uma abaixo da outra, vamos exibir as 5 mais recentes e as 5 maiores, colocando um link em cada para mais detalhes.

Para fazer o processo de carregar essas duas listas de Sales e o Seller, vamos criar uma nova ViewModel, vou chamar de `SellerDetailsViewModel`.

```c#

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

```

Note como o Seller é uma propriedade comum (isso porque quando criarmos essa viewModel, passaremos qual é o Seller) mas a propriedade RecentSales e a Biggest Sales (vendas recentes e maiores vendas respectivamente) são propriedades somente para leitura, isso porque elas tem seus valores definidos com base nos métodos abaixo delas, esses métodos essencialmente pegam o Seller que passamos, ordenam as vendas de forma decrescente por data (no RecentSales) ou por valor (no BiggestSales) e usaam o `Take(5)` para selecionar os 5 primeiros, é o LINQ em ação.

## Details do Controller

Agora iremos criar a Action que busca o Seller específico, cria a ViewModel que fizemos antes e manda para a View correspondente:

```c#
// GET: Sellers/Details/x
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

    SellerDetailsViewModel viewModel = new SellerDetailsViewModel
    {
        Seller = obj
    };

    return View(viewModel);
}
```

## FindByIdEagerAsync do Service

```c#
public async Task<Seller> FindByIdEagerAsync(int id)
{
    return await _context.Sellers.Include(x => x.Sales).ThenInclude(x => x.Books).FirstOrDefaultAsync(x => x.Id == id);
}
```

Note que usamos pela primeira vez o método ThenInclude, isso porque queremos buscar as vendas do vendedor, mas para saber o valor da venda, precisamos saber o valor dos livros dela, logo, precisamos carregar o vendedor, suas vendas e os livros dessas vendas.

Exemplo de dados importados
```
Vendedor > Venda1 > Livro1 - 50$
         > Venda2 > Livro2 - 75$
                  > Livro3 - 100$
         > Venda3 > Livro1 - 50$
...
```

## View Details

```html
@model Bookstore.Models.ViewModels.SellerDetailsViewModel

@{
    ViewData["Title"] = Model.Seller.Name;
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<div>
    <h4 class="lead text-secondary-emphasis">Detalhes do Vendedor</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Name)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Name)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Email)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Email)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Salary)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Salary)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Total de Vendas
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Model.Seller.CalculateTotalSalesAmount().ToString("C2")
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Vendas mais recentes
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Sale sale in Model.RecentSales)
            {
                <a asp-controller="Sales" asp-action="Details" asp-route-id="@sale.Id" class="text-primary">@sale.Date.ToString("dd/MM/yyyy") | @sale.Amount.ToString("C2")</a>
                <br />
            }
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Maiores vendas
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Sale sale in Model.BiggestSales)
            {
                <a asp-controller="Sales" asp-action="Details" asp-route-id="@sale.Id" class="text-primary">@sale.Date.ToString("dd/MM/yyyy") | @sale.Amount.ToString("C2")</a>
                <br />
            }
        </dd>

    </dl>
</div>
<div>
    <a asp-action="Edit" asp-route-id="@Model?.Seller.Id" class="text-primary">Editar</a> |
    <a asp-action="Index" class="text-primary">Voltar para a lista</a>
</div>

```

## Delete GET do Controller

No delete também mostraremos detalhes sobre o vendedor, então também usaremos essa viewModel que criamos.

```c#
 // GET: Sellers/Delete/5
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

     SellerDetailsViewModel viewModel = new SellerDetailsViewModel
     {
         Seller = obj
     };

     return View(viewModel);
 }
```
O FindByIdEagerAsync do Service já foi criado para usar no Details, então não precisamos mudar mais nada.

## View Delete

```html
@model Bookstore.Models.ViewModels.SellerDetailsViewModel

@{
    ViewData["Title"] = "Apagar Vendedor";
}

<h1 class="text-primary-emphasis">@ViewData["Title"]</h1>

<h3 class="lead text-secondary-emphasis mb-4">Tem certeza que deseja apagar este vendedor?</h3>
<div>
    <h4 class="text-primary-emphasis">Vendedor</h4>
    <hr />
    <dl class="row">
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Name)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Name)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Email)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Email)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            @Html.DisplayNameFor(model => model.Seller.Salary)
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Html.DisplayFor(model => model.Seller.Salary)
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Total de Vendas
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @Model.Seller.CalculateTotalSalesAmount().ToString("C2")
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Vendas mais recentes
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Sale sale in Model.RecentSales)
            {
                <a asp-controller="Sales" asp-action="Details" asp-route-id="@sale.Id" class="text-primary">@sale.Date.ToString("dd/MM/yyyy") | @sale.Amount.ToString("C2")</a>
                <br />
            }
        </dd>
        <dt class="col-sm-2 text-primary-emphasis">
            Maiores vendas
        </dt>
        <dd class="col-sm-10 text-primary-emphasis">
            @foreach (Sale sale in Model.BiggestSales)
            {
                <a asp-controller="Sales" asp-action="Details" asp-route-id="@sale.Id" class="text-primary">@sale.Date.ToString("dd/MM/yyyy") | @sale.Amount.ToString("C2")</a>
                <br />
            }
        </dd>
    </dl>
    
    <form asp-action="Delete">
        <input type="hidden" asp-for="Seller.Id" />
        <input type="submit" value="Apagar" class="btn btn-outline-danger" /> |
        <a asp-action="Index" class="text-primary">Voltar para a lista</a>
    </form>>
</div>
```

## Delete POST do Controller

```c#
// POST: Sellers/Delete/5
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
        var obj = await _context.Sellers.FindAsync(id);
        _context.Sellers.Remove(obj);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        throw new IntegrityException(ex.Message);
    }
}
```

## Error do Controller

Por fim, falta só a Action de erro, que é igual em todos os controllers:

```c#
// GET Sellers/Error
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
