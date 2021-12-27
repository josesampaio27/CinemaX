using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaX.Data;
using CinemaX.Models;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace CinemaX.Controllers
{
    public class BackOfficeController : Controller
    {
        private readonly CinemaXContext _context;
        private readonly IHostEnvironment _he;
        private readonly INotyfService _notyf;

        public BackOfficeController(CinemaXContext context, IHostEnvironment e, INotyfService notyf)
        {
            _context = context;
            _he = e;
            _notyf = notyf;
        }

        // GET: BackOffice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.Include(u => u.CategoriasFilmes)
                .FirstOrDefaultAsync(m => m.IdFilme == id);

            foreach(var cat in filme.CategoriasFilmes)
            {               
                filme.CategoriasFilmes.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdFilme == cat.IdFilme).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
            }

            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: BackOffice/Create
        public IActionResult AddMovie()
        {
            if(!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            ViewBag.Categorias = _context.Categoria;
            return View();  
        }

        public IActionResult PermissionDenied()
        {           
            return View();
        }

        public IActionResult Index()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            return View();
        }


        // POST: BackOffice/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie([Bind("IdFilme,Nome,Foto,Realizador,Data,LinkTrailer,Descrição,Duracao,IdCreationUser,DataAdicionado")] Filme filme, IFormFile Foto,int []IdCategorias)
        {
            filme.Foto = Foto.FileName;
         
            if (ModelState.IsValid)
            {

                string destination = Path.Combine(
                    _he.ContentRootPath, "wwwroot/Fotos/", Path.GetFileName(Foto.FileName)
                    );


                FileStream fs = new FileStream(destination, FileMode.Create);

                Foto.CopyTo(fs);
                fs.Close();

                foreach(int catid in IdCategorias)
                {
                    CategoriasFilme aux = new CategoriasFilme();
                    Categorium cat = _context.Categoria.FirstOrDefault(c => c.IdCategoria == catid);
                    aux.IdCategoria = catid;
                    aux.IdCategoriaNavigation = cat;
                    aux.IdFilme = filme.IdFilme;
                    aux.IdFilmeNavigation = filme;

                    _context.Add(aux);
                }

                filme.IdCreationUser = (int)HttpContext.Session.GetInt32("IdUtilizador");

                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = _context.Categoria;
            return View(filme);
        }

        // GET: BackOffice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null)
            {
                return NotFound();
            }
            return View(filme);
        }

        // POST: BackOffice/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFilme,Nome,Foto,Realizador,Data,LinkTrailer,Descrição,Duracao,IdCreationUser,DataAdicionado")] Filme filme)
        {
            if (id != filme.IdFilme)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.IdFilme))
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
            return View(filme);
        }

        // GET: BackOffice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.IdFilme == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: BackOffice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.IdFilme == id);
        }

        public bool Perm(int perm)
        {
            string aux = HttpContext.Session.GetString("Permissoes");

            if (aux == null || !aux.Contains(perm.ToString()))
                return false;

            return true;
        }

    }
}
