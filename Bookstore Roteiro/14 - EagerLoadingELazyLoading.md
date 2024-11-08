# Eager vs Lazy


## Cenário exemplo 

Imagine o seguinte, você quer carregar um gênero do banco de dados para exibir os dados do gênero na tela.

Mas esse gênero possui uma lista de livros.

Você carrega esses livros também ou só carrega o gênero?

Se você decidiu carregar os livros, também tem que ter em mente que cada livro tem uma lista de vendas, carregamos elas também?

No começo era só um gênero, agora são 10 livros e 60 vendas carregando, isso com certeza vai ser um gasto desnecessário de processamento no banco de dados se você não vai usar tudo isso.

Portanto, precisamos saber sempre se precisamos puxar mais do que só a entidade base nas nossas consultas.

## Nosso problema

Até agora, para criar e editar um gênero, bem como para ver a lista de gêneros, não precisávamos carregar quais são os livros acoplados a eles, já que essas informações não iriam aparecer.
 
Mas para dar uma utilidade à funcionalidade de Details, vamos querer que, para essa consulta, sejam exibidos os livros de cada gênero.

## Conceitos

O nome para isso é ***Eager Loading***, ou ***Carregamento Ansioso***, isso porque carregamos a entidade principal e também as derivadas dela.

Por padrão, o carregamento do EF Core é ***Lazy Loading***, ou ***Carregamento Preguiçoso***, nesse tipo, só é carregada a entidade principal, as com quem ela se relaciona não.

## Nosso carregamento atual

```c#
public async Task<Genre> FindByIdAsync(int id)
{
    return await _context
        .Genres
        .FirstOrDefaultAsync(x => x.Id == id);
}
```

Note a estrutura do LINQ com EF Core.

"No contexto (Banco de dados), carregue na tabela Genres, o primeiro registro cujo Id seja igual ao id passado para esse método, se não, retorna o valor padrão (null)".

Precisamos adaptar para algo como

"No contexto (Banco de dados), carregue na tabela Genres, **INCLUINDO TAMBÉM OS LIVROS**, o primeiro registro cujo Id seja igual ao id passado para esse método, se não retorna o valor padrão (null)."

Para fazer esse "incluindo também os livros", basta fazer isso:

```c#
return await _context
    .Genres
    .Include(x => x.Books)
    .FirstOrDefaultAsync(x => x.Id == id);
```

Nesse x => x.Books estamos dizendo: "ok, você está carregando algo da tabela de gêneros? Essa tabela também tem conexão com um atributo chamado Books, pega ele também."

Mas não vamos simplesmente substituir o método antigo pelo novo, esse método de **FindByIdAsync** será usado em outros momentos em que não é necessário puxar seus livros junto, logo, é mais interessante criar os dois métodos, o para quando queremos os livros juntos e o para quando não queremos.

Como o padrão é o Lazy Loading, vamos manter o **FindByIdAsync** sem o Include() e criar outro chamado **FindByIdEagerAsync** contendo o Include().

```c#
// GET: Genres/Details/x
public async Task<Genre> FindByIdEagerAsync(int id)
{
    return await _context
        .Genres
        .Include(x => x.Books)
        .FirstOrDefaultAsync(x => x.Id == id);
}


public async Task<Genre> FindByIdAsync(int id)
{
    return await _context
        .Genres
        .FirstOrDefaultAsync(x => x.Id == id);
}
```

E lá na Action Details do GenreController, vamos chamar o FindByEagerAsync(), já que NESSE CASO, queremos carregar também os livros.
