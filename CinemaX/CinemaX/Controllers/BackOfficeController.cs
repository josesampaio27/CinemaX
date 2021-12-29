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

        public async Task<IActionResult> FilmList()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            return View(await _context.Filmes.ToListAsync());
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


        // POST: BackOffice/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie([Bind("IdFilme,Nome,Foto,Realizador,Data,LinkTrailer,Descrição,Duracao,IdCreationUser,DataAdicionado")] Filme filme, IFormFile Foto,int []IdCategorias)
        {
            if (Foto == null)
                ModelState.AddModelError("Foto", "É obrigatorio introduzir uma foto");

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
                return RedirectToAction(nameof(FilmList));
            }

            ViewBag.Categorias = _context.Categoria;
            return View(filme);
        }

        // GET: BackOffice/Edit/5
        public async Task<IActionResult> EditMovie(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            var filme =  _context.Filmes.Include(f => f.CategoriasFilmes).FirstOrDefault(f => f.IdFilme == id);

            if (filme == null)
            {
                return NotFound();
            }

            foreach (CategoriasFilme catf in _context.CategoriasFilmes)
            {
                if(_context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == id && f.IdCategoria == catf.IdCategoria) != null)
                    filme.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == id && f.IdCategoria == catf.IdCategoria).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == catf.IdCategoria);
            }

            ViewBag.Categorias = _context.Categoria;

            return View(filme);
        }

        // POST: BackOffice/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(int id, [Bind("IdFilme,Nome,Foto,Realizador,Data,LinkTrailer,Descrição,Duracao,IdCreationUser,DataAdicionado")] Filme filme ,IFormFile Foto, int[] IdCategorias)
        {
            if (id != filme.IdFilme)
            {
                return NotFound();
            }           

            if (ModelState.IsValid)
            {
                foreach (CategoriasFilme catf in _context.CategoriasFilmes)
                {
                    if (catf.IdFilme == id)
                        _context.Remove(catf);
                }

                foreach (int catid in IdCategorias)
                {
                    CategoriasFilme aux = new CategoriasFilme();
                    Categorium cat = _context.Categoria.FirstOrDefault(c => c.IdCategoria == catid);
                    aux.IdCategoria = catid;
                    aux.IdCategoriaNavigation = cat;
                    aux.IdFilme = id;                  
                    _context.Add(aux);
                }

                Filme flm = _context.Filmes.FirstOrDefault(f => f.IdFilme == filme.IdFilme);
                

                if (Foto != null)
                {
                     flm.Foto = Foto.FileName;

                     string destination = Path.Combine(
                    _he.ContentRootPath, "wwwroot/Fotos/", Path.GetFileName(Foto.FileName)
                    );


                    FileStream fs = new FileStream(destination, FileMode.Create);

                    Foto.CopyTo(fs);
                    fs.Close();
                }

                flm.Data = filme.Data;
                flm.Descrição = filme.Descrição;
                flm.Duracao = filme.Duracao;
                flm.LinkTrailer = filme.LinkTrailer;
                flm.Nome = filme.Nome;
                flm.Realizador = filme.Realizador;               
               
                try
                {                    
                    _context.Update(flm);
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
                return RedirectToAction(nameof(FilmList));
            }

            filme.CategoriasFilmes = _context.Filmes.FirstOrDefault(f => f.IdFilme == id).CategoriasFilmes;

            foreach (CategoriasFilme catf in _context.CategoriasFilmes)
            {
                if (_context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == id && f.IdCategoria == catf.IdCategoria) != null)
                    filme.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == id && f.IdCategoria == catf.IdCategoria).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == catf.IdCategoria);
            }

            ViewBag.Categorias = _context.Categoria;

            return View(filme);
        }

        public async Task<IActionResult> MovieDetails(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

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

        // GET: BackOffice/Delete/5
        public async Task<IActionResult> DeleteMovie(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

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
        [HttpPost, ActionName("DeleteMovie")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            foreach (CategoriasFilme catf in _context.CategoriasFilmes)
            {
                if (catf.IdFilme == id)
                    _context.Remove(catf);
            }

            var filme = await _context.Filmes.FindAsync(id);
            _context.Filmes.Remove(filme);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(FilmList));
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
