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
using System.Threading;
using System.Globalization;

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

        // GET: BackOffice/FilmList
        public async Task<IActionResult> FilmList()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: BackOffice/Rooms
        public async Task<IActionResult> Rooms()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            return View(await _context.Salas.ToListAsync());
        }

        // GET: BackOffice/AddMovie
        public IActionResult AddMovie()
        {
            if(!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            ViewBag.Categorias = _context.Categoria;
            return View();  
        }

        // GET: BackOffice/PermissionDenied
        public IActionResult PermissionDenied()
        {           
            return View();
        }

        public async Task<IActionResult> SessionList()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            return View(await _context.Sessaos.Include(f => f.IdFilmeNavigation).Include(f=> f.NumeroNavigation).ToListAsync());
        }

        // GET: BackOffice/AddSession
        public IActionResult AddSession()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));


            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "Numero","Numero");           
            return View();
        }

        //POST: BackOffice/AddSession
        [HttpPost]
        public async Task<IActionResult> AddSession([Bind("IdFilme,Numero,Data,Preço_string")] Sessao sessao)
        {
            
            if (ModelState.IsValid)
            {
                sessao.Preço = decimal.Parse(sessao.Preço_string, CultureInfo.InvariantCulture);

                Filme filme;
                Sala sala;

                if ( (filme = _context.Filmes.FirstOrDefault(f=>f.IdFilme == sessao.IdFilme)) == null)
                    return NotFound();

                if ((sala = _context.Salas.FirstOrDefault(s => s.Numero == sessao.Numero)) == null)
                    return NotFound();

                sessao.IdFilmeNavigation = filme;
                sessao.NumeroNavigation = sala;

                sessao.Vagas = sala.Capacidade;

                _context.Add(sessao);
                await _context.SaveChangesAsync();
                _notyf.Success("Sessao adicionada com sucesso");
                return RedirectToAction(nameof(FilmList));
            }


            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "Numero", "Numero");
            return View(sessao);
        }

        // GET: BackOffice/EditSession
        public IActionResult EditSession(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            var sessao = _context.Sessaos.FirstOrDefault(s => s.IdSessao == id);

            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "Numero", "Numero");

            sessao.Preço_string = sessao.Preço.ToString().Replace(",",".");
            sessao.Preço_string = sessao.Preço_string.Remove(sessao.Preço_string.Length - 2, 2);

            return View(sessao);
        }

        //POST: BackOffice/EditSession
        [HttpPost]
        public async Task<IActionResult> EditSession(int id, [Bind("IdSessao,IdFilme,Numero,Data,Preço_string")] Sessao sessao)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id != sessao.IdSessao)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                sessao.Preço = decimal.Parse(sessao.Preço_string, CultureInfo.InvariantCulture);

                Filme filme;
                Sala sala;

                if ((filme = _context.Filmes.FirstOrDefault(f => f.IdFilme == sessao.IdFilme)) == null)
                    return NotFound();

                if ((sala = _context.Salas.FirstOrDefault(s => s.Numero == sessao.Numero)) == null)
                    return NotFound();

                sessao.IdFilmeNavigation = filme;
                sessao.NumeroNavigation = sala;

                sessao.Vagas = sala.Capacidade;

                _context.Update(sessao);
                await _context.SaveChangesAsync();
                _notyf.Success("Sessao editada com sucesso");
                return RedirectToAction(nameof(SessionList));
            }


            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "Numero", "Numero");
            return View(sessao);
        }

        // GET: BackOffice/DeleteSession
        public IActionResult DeleteSession(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if(id == null)
                return NotFound();

            var sessao = _context.Sessaos.Find(id);

            sessao.IdFilmeNavigation = _context.Filmes.Find(sessao.IdFilme);
            sessao.NumeroNavigation = _context.Salas.Find(sessao.Numero);

            return View(sessao);
        }

        // POST: BackOffice/DeleteSession 
        [HttpPost, ActionName("DeleteSession")]
        public async Task<IActionResult> DeleteSessionConfirmed(int id)
        {
            var sessao = await _context.Sessaos.FindAsync(id);
            _context.Sessaos.Remove(sessao);
            await _context.SaveChangesAsync();
            _notyf.Success("Sessao elimiada com sucesso");
            return RedirectToAction(nameof(SessionList));
        }

        // GET: BackOffice/AddRoom
        public IActionResult AddRoom()
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));
            return View();
        }

        //POST: BackOffice/AddRoom
        [HttpPost]
        public async Task<IActionResult> AddRoom([Bind("Capacidade")] Sala sala)
        {
            if (ModelState.IsValid)
            {
                sala.DataAdicionada = DateTime.Now;
                sala.IdCreationUser = (int)HttpContext.Session.GetInt32("IdUtilizador");
                _context.Add(sala);
                await _context.SaveChangesAsync();
                _notyf.Success("sala adicionada com sucesso");
                return RedirectToAction(nameof(Rooms));
            }
            return View(sala);
        }

        // GET: BackOffice/EditRoom/?
        public IActionResult EditRoom(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            var sala = _context.Salas.FirstOrDefault(S => S.Numero == id);

            if (sala == null)
            {
                return NotFound();
            }           
          
            return View(sala);      
        }

        // POST: BackOffice/EditRoom
        [HttpPost]
        public async Task<IActionResult> EditRoom(int id, [Bind("Capacidade")] Sala sala)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (ModelState.IsValid)
            {
                Sala _sala = _context.Salas.FirstOrDefault(s => s.Numero == id);
                _sala.Capacidade = sala.Capacidade;
                _context.Update(_sala);
                await _context.SaveChangesAsync();
                _notyf.Success("sala editada com sucesso");
                return RedirectToAction(nameof(Rooms));
            }

            return View(sala);
        }

        // GET: BackOffice/DeleteRoom/?
        public async Task<IActionResult> DeleteRoom(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            if (_context.Salas.FirstOrDefault(s => s.Numero == id) != null)
            {
                _notyf.Error("Impossivel eliminar sala se esta ainda contiver sessoes");
                return RedirectToAction(nameof(Rooms));
            }

            var sala = await _context.Salas.FirstOrDefaultAsync(s => s.Numero == id);
            
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // POST: BackOffice/DeleteRoom 
        [HttpPost, ActionName("DeleteRoom")]       
        public async Task<IActionResult> DeleteRoomConfirmed(int id)
        {          
            var sala = await _context.Salas.FindAsync(id);
            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();
            _notyf.Success("sala eliminada com sucesso");
            return RedirectToAction(nameof(Rooms));
        }


        // POST: BackOffice/AddMoive        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie([Bind("Nome,Foto,Realizador,Data,LinkTrailer,Descrição,Duracao,DataAdicionado")] Filme filme, IFormFile Foto,int []IdCategorias)
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
                _notyf.Success("filme adicionado com sucesso");
                return RedirectToAction(nameof(FilmList));
            }

            ViewBag.Categorias = _context.Categoria;
            return View(filme);
        }

        // GET: BackOffice/EditMovie/?
        public IActionResult EditMovie(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            var filme =  _context.Filmes.FirstOrDefault(f => f.IdFilme == id);

            if (filme == null)
            {
                return NotFound();
            }

            foreach (CategoriasFilme catf in _context.CategoriasFilmes)
            {
                if (_context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == id && f.IdCategoria == catf.IdCategoria) != null)
                {
                    catf.IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == catf.IdCategoria);
                    filme.CategoriasFilmes.Add(catf);
                }
            }

            ViewBag.Categorias = _context.Categoria;

            return View(filme);
        }

        // POST: BackOffice/EditMovie/?       
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
                _notyf.Success("filme editado com sucesso");
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

        // GET: BackOffice/MovieDetails
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

        // GET: BackOffice/DeleteMovie/5
        public async Task<IActionResult> DeleteMovie(int? id)
        {
            if (!Perm(1))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            if(_context.Sessaos.FirstOrDefault(s=>s.IdFilme == id) != null)
            {
                _notyf.Error("Impossivel eliminar filme se este ainda contiver sessoes");
                return RedirectToAction(nameof(FilmList));
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
            _notyf.Success("Filme eleminado com sucesso");
            return RedirectToAction(nameof(FilmList));
        }

        //Function
        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.IdFilme == id);
        }

        //Function
        public bool Perm(int perm)
        {
            string aux = HttpContext.Session.GetString("Permissoes");

            if (aux == null || !aux.Contains(perm.ToString()))
                return false;

            return true;
        }

    }
}
