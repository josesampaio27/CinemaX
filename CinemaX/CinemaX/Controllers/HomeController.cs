using CinemaX.Data;
using CinemaX.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CinemaXContext _context;

        public HomeController(ILogger<HomeController> logger, CinemaXContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Filmes.FirstOrDefault(f => f.IdFilme == id) == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.Include(u => u.CategoriasFilmes)
                .FirstOrDefaultAsync(m => m.IdFilme == id);

            foreach (var cat in filme.CategoriasFilmes)
            {
                filme.CategoriasFilmes.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdFilme == cat.IdFilme).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
            }

            return View(filme);
        }

        public async Task<IActionResult> Filmes()
        {
            #region Data preparation
            List<string> NomeFilmes = new List<string>();
            List<string> NomeRealizador = new List<string>();

            foreach(var filme in _context.Filmes)
            {
                NomeFilmes.Add(filme.Nome);
                if(!NomeRealizador.Contains(filme.Realizador))
                    if(!filme.Realizador.Contains(","))
                        NomeRealizador.Add(filme.Realizador);
                    else
                    {
                        if (!NomeRealizador.Contains(filme.Realizador.Split(",")[0]))                       
                            NomeRealizador.Add(filme.Realizador.Split(",")[0]);
                        if (!NomeRealizador.Contains(filme.Realizador.Split(",")[1]))
                            NomeRealizador.Add(filme.Realizador.Split(",")[1]);
                        
                    }
            }

            ViewBag.NomeFilmes = new HtmlString(JsonConvert.SerializeObject(NomeFilmes.ToArray()));
            ViewBag.NomeRealizador = new HtmlString(JsonConvert.SerializeObject(NomeRealizador.ToArray()));
            ViewBag.Filmes = _context.Filmes;
            ViewBag.Categorias = _context.Categoria;
            #endregion
            return View(await _context.Categoria.ToListAsync());
        }

        public IActionResult Procurar(string NomeFilme, string NomeRealizador, int[] IdCategorias)
        {
            List<Filme> filmes = new List<Filme>();

            if (NomeFilme == null && NomeRealizador == null && IdCategorias.Length == 0)
            {
                ViewBag.Filmes = _context.Filmes;
            }

            else if(NomeRealizador == null && IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeFilme == null && IdCategorias.Length == 0)
            {
                foreach(var film in _context.Filmes)
                {
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }
            else if (NomeFilme == null && NomeRealizador == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f=>f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if(filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }
            else if(IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeFilme == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeRealizador == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            return PartialView("Filme");
        }
    }
}
