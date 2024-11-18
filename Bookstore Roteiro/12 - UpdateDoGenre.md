# Update do Genre

O método de Update é bem similar ao de Delete, os passos continuam os mesmos:

1 - Criar Action que carrega a View de editar

2 - Criar a view de Editar

3 - Criar a Action que realmente mandar o service editar

4 - Criar o método no Service que faz a edição no banco de dados

5 - Verificar se as exceções estão certas.

## 1 - Action que carrega a View
```c#
// GET Genres/Edit/x
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

Note que tá tudo igual ao de apagar, vê se o Id tá vazio e se existe um objeto com esse Id, se um desses não deu certo, vai pra tela de erro, se deram, carrega a View passando o objeto que será editado.

## 2 - View de edição

Em termos de estética, a view de editar vai ser basicamente igual a view de criar um Genre, com a diferença principal que os `inputs` já virão preenchidos com o valor atual daquele campo.

Ou seja, quando iriamos criar um Genre, o input do Nome começava vazio, agora que iremos editar, ele começa já o nome atual do Genre dentro, basicamente só isso.

```html
@model Bookstore.Models.Genre

@{
    ViewData["Title"] = "Editar Gênero Literário";
}

<h1>@ViewData["Title"]</h1>

<h4 class="lead">Altere as informações desejadas abaixo</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="Edit">
            <!-- Esse input oculto contribui para que a requisição de edição seja feita apropriadamente -->
            <input type="hidden" asp-for="Id" />
            <div class="form-group">
                <label asp-for="Name" class="control-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Salvar Alterações" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Voltar para a lista</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```
Uma outra pequena diferença é o Id sendo colocado de forma oculta num input, isso ajuda a, quando o formulário sofrer submit, ele conter todos os dados do Genre.

## 3 - Action que manda o Service editar

Essa agora é uma mistura da action de POST para criar e de POST para apagar:

```c#
// POST Genres/Edit/x
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Genre genre)
{
    if (!ModelState.IsValid)
    {
        return View();
    }

    if (id != genre.Id) {
        return RedirectToAction(nameof(Error), new { message = "Id's não condizentes" });
    }

    try
    {
        await _service.UpdateAsync(genre);
        return RedirectToAction(nameof(Index));
    }
    catch (ApplicationException ex)
    {
        return RedirectToAction(nameof(Error), new { message = ex.Message });
    }
}
```

Se o dado preeenchido não bater com os requisitos, carrega a mesma view de novo.

Se o id passado na URL não bate com o do formulário, carrega a view de erro (isso pode acontecer se o cara tentar forçar uma requisição fora da nossa view, tipo por terminal e acabar passando o id errado).

Tenta atualizar os dados através do service passando o objeto montado naquele formulário que veio no parâmetro do método, se funcionar vai pra Action Index, se não funcionar redireciona pra tela de erro passando a mensagem da exceção.

## 4 - Método do Service que atualiza
```c#
// POST: Genres/Edit/x
public async Task UpdateAsync(Genre genre)
{
    // Confere se tem alguém com esse Id
    bool hasAny = await _context.Genres.AnyAsync(x => x.Id == genre.Id);
    // Se não tiver, lança exceção de NotFound.
    if (!hasAny)
    {
        throw new NotFoundException("Id não encontrado");
    }
    // Tenta atualizar
    try
    {
        _context.Update(genre);
        await _context.SaveChangesAsync();
    }
    // Se não der, captura a exceção lançada
    catch (DbUpdateConcurrencyException ex)
    {
        throw new DbConcurrencyException(ex.Message);
    }
}
```

## 5 - Verificar se as exceções estão certas

Essa exceção de DbUpdateConcurrency é super específica, ela ocorre quando um usuário atualiza os dados e outro também antes da atualização do primeiro terminar, mas é bom prevenir, além disso, note que tanto a exceção `NotFoundException` quanto a `DbConcurrencyException` não existem, precisamos criar elas e é da exata mesma forma que criamos a exceção de Integrity, não vou nem mostrar aqui.


