using CinemaX.Data;
using CinemaX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    }
}
