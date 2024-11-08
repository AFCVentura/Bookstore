# Criando o BookService

O primeiro passo é criar a classe BookService como vocês já fizeram com o Genre e depois adicionar ela na lista de Services.

## Vinculando o Controller ao Service e o Service ao Context

Como dito anteriormente, nosso Controller está acessando diretamente o Context, agora que adicionamos a classe Service, precisamos conectar o **Controller** a ela:

```c#
// Antes
public class BooksController : Controller
{
    private readonly BookstoreContext _context;

    public BooksController(BookstoreContext context)
    {
        _context = context;
    }
...
```

```c#
//Depois
public class BooksController : Controller
{
    private readonly BookService _service;

    public BooksController(BookService service)
    {
        _service = service;
    }
...
```

Já no **Service**, aí sim conectamos com o Context:

```c#
public class BookService
{
    private readonly BookstoreContext _context;

    public BookService(BookstoreContext context)
    {
        _context = context;
    }
...
```

Agora precisamos começar o processo de converter todos os métodos para Async e também de mover a modificação do banco de dados para o service.

### Action Index

```c#
// Antes no Controller

// GET: Books
public async Task<IActionResult> Index()
{
    return View(await _context.Books.ToListAsync());
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// GET: Books
public async Task<IActionResult> Index()
{
    return View(await _service.FindAllAsync());
}

----------------------------------

// Depois no Service

// GET Books/Index
public async Task<List<Book>> FindAllAsync()
{
    return await _context.Books.ToListAsync();
}
```

Mais pra frente vamos precisar fazer uma alteração nesse método relacionada a Eager Loading, mas vamos deixar assim por enquanto.

### Action Create (GET)

Esse método POR ENQUANTO ficará igual, depois terá bastante mudanças, mas no que tange a conectar-se com o service, ele já não se conectava com o banco antes, então se mantém assim.

### Action Create (POST)

```c#
// Antes no Controller

// POST: Books/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Id,Title,Price,Author,ReleaseYear")] Book book)
{
    if (ModelState.IsValid)
    {
        _context.Add(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(book);
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// POST: Books/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Book book) {
    if (!ModelState.IsValid) {
        return View();
    }

    await _service.InsertAsync(book);
    return RedirectToAction(nameof(Index));
}

----------------------------------

// Depois no Service
public async Task InsertAsync(Book book)
{
    _context.Add(book);
    await _context.SaveChangesAsync();
}
```

Note que invertemos a lógica, antes faziamos a checagem se deu certo no if, agora fazemos a se deu errado e a lógica principal fora. Uma recomendação de boa prática em basicamente qualquer linguagem é evitar ficar aninhando código (colocando bloco dentro de bloco dentro de bloco), então deixamos a parte com mais código fora do if.

Além disso removemos a anotação Bind, essa anotação serve para especificar o que vai ser passado da View para o Controller, mas é opcional e só vai nos atrapalhar depois.

Esse método também vai sofrer mudanças mais pra frente.

### Action Edit (GET)

```c#
// Antes no Controller

// GET: Books/Edit/5
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var book = await _context.Books.FindAsync(id);
    if (book == null)
    {
        return NotFound();
    }
    return View(book);
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// GET: Books/Edit/x
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

----------------------------------

// Depois no Service

public async Task<Book> FindByIdAsync(int id)
{
    return await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
}
```

Aqui a diferença mais significativa foi o redirecionamento para nossa tela de erro personalizada que já tinhamos criado antes, além é claro de delegar a busca do livro para o service.

### Action Edit (POST)

```c#
// Antes no Controller

// POST: Books/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Price,Author,ReleaseYear")] Book book)
{
    if (id != book.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(book.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }
    return View(book);
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// POST: Books/Edit/x
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Book book)
{
    if (!ModelState.IsValid)
    {
        return View();
    }
    if (id != book.Id)
    {
        return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
    }

    try
    {
        await _service.UpdateAsync(book);
        return RedirectToAction(nameof(Index));
    }
    catch (ApplicationException ex)
    {
        return RedirectToAction(nameof(Error), new { message = ex.Message });
    }
}

----------------------------------

// Depois no Service

public async Task UpdateAsync(Book book)
{
    bool hasAny = await _context.Books.AnyAsync(x => x.Id == book.Id);
    if (!hasAny)
    {
        throw new NotFoundException("Id não encontrado");
    }

    try
    {
        _context.Update(book);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        throw new DbConcorrencyException(ex.Message);
    }
}

```

Novamente, essa tela também vai mudar bastante.

### Action Delete (GET)
```c#
// Antes no Controller

// GET: Books/Delete/5
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var book = await _context.Books
        .FirstOrDefaultAsync(m => m.Id == id);
    if (book == null)
    {
        return NotFound();
    }

    return View(book);
}

----------------------------------

// Antes no Service


```

```c#
// Depois no Controller

// GET: Books/Delete/x
public async Task<IActionResult> Delete(int? id)
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


----------------------------------

// Depois no Service

// Método FindByIdAsync que criamos antes

```

Aqui a diferença além de ter delegado a função foi uma leve refatoração no código, usando a expressão is null ao invés da verificação comum, as diferenças não são tão grandes mas é uma forma mais moderna de verificação nesse caso, assim como o contrário poderia ser feito com 'is not null'.

### Action Delete (POST)

```c#
// Antes no Controller

// POST: Books/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var book = await _context.Books.FindAsync(id);
    if (book != null)
    {
        _context.Books.Remove(book);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// POST: Books/Delete/x
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

----------------------------------

// Depois no Service

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
```

Aqui mudamos o nome da Action pois o código gerado sugere mudar o nome dela no código e usar uma anotação ActionName para voltar pra Delete, mas no nosso caso não é necessário (ele sugere isso para evitar problemas de Actions com o mesmo nome).

Além disso, implementamos o sistema de lançamento de exceções no Service, ele captura a exceção do banco, emite outra que é capturada pelo controller que redireciona para uma tela de erro.

### Action Details (GET)

```c#
// Antes no Controller

// GET: Books/Details/5
public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var book = await _context.Books
        .FirstOrDefaultAsync(m => m.Id == id);
    if (book == null)
    {
        return NotFound();
    }

    return View(book);
}

----------------------------------

// Antes no Service

```

```c#
// Depois no Controller

// GET: Books/Details/x
public async Task<IActionResult> Details(int? id)
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

----------------------------------

// Depois no Service

// Método FindByIdAsync que criamos antes
```

Esse também teve alterações menores e usou um método que havíamos criado anteriormente.

### Action BookExists

Essa action não será usada, podemos apagar ela.

### Action Error

Nosso código não tem uma Action que será essencial, a de carregar a tela de erro, ela será basicamente igual a que temos no Genre:


```c#
// GET Genres/Error
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

## Conclusão

Nessa etapa, criamos o Service e adicionamos os métodos que serão responsáveis por manipular o banco de dados, já no controlador, removemos essa responsabilidade e refatoramos um pouco o código. Se vocês quiserem testar, no entanto, notarão alguns problemas.

1. As views estão em inglês e sem os ajustes de estética nossos.
2. Não conseguimos escolher nem editar os gêneros literários dos livros.

Esse segundo erro é o mais sério, pois a relação dos dois é fundamental para possibilitar buscar livros pelo seu gênero.

