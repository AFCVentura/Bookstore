using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookstore.Data;
using Bookstore.Models;
using Bookstore.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Bookstore.Services.Exceptions;
using Bookstore.Models.ViewModels;
using System.Diagnostics;

namespace Bookstore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookService _service;
        private readonly GenreService _genreService;

        public BooksController(BookService service, GenreService genreService)
        {
            _service = service;
            _genreService = genreService;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {

            return View(await _service.FindAllAsync());
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            List<Genre> genres = await _genreService.FindAllAsync();

            BookFormViewModel viewModel = new BookFormViewModel { Genres = genres };
            return View(viewModel);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel viewModel)
        {
            // Se nem todos os dados da viewModel estão preenchidos corretamente
            if (!ModelState.IsValid)
            {
                // Carrega os gêneros para eles aparecerem na lista de exibição
                viewModel.Genres = await _genreService.FindAllAsync();
                // E recarrega a página de Create
                return View(viewModel);
            }

            // caso esteja tudo certo, cria uma lista no atributo Genres do livro criado
            viewModel.Book.Genres = new List<Genre>();

            // Percorre todos os ids que foram selecionados pelo usuário na lista
            foreach (int genreId in viewModel.SelectedGenresIds)
            {
                // Busca um de cada vez no banco de dados
                Genre genre = await _genreService.FindByIdAsync(genreId);
                // Se de fato existe um gênero com esse Id
                if (genre is not null)
                {
                    // Adiciona esse gênero na lista do livro
                    viewModel.Book.Genres.Add(genre);
                }
            }
            // Em essência, pega todos os ids selecionados, busca os gêneros no banco
            // e vincula eles ao livro que vai ser criado

            // Insere o novo livro com esses dados vinculados.
            await _service.InsertAsync(viewModel.Book);

            // Redireciona para a tela que exibe todos os livros.
            return RedirectToAction(nameof(Index));
        }

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

            List<Genre> genres = await _genreService.FindAllAsync();
            BookFormViewModel viewModel = new BookFormViewModel { Book = obj, Genres = genres };

            return View(viewModel);
        }

        // POST: Books/Edit/x
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (id != viewModel.Book.Id)
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
    }
}