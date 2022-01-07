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
using System.Security.Cryptography;
using CinemaX.Services;

namespace CinemaX.Controllers
{
    public class BackOfficeController : Controller
    {
        private readonly CinemaXContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHostEnvironment _he;
        private readonly INotyfService _notyf;
        static readonly char[] availableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
  };

        public BackOfficeController(CinemaXContext context, IHostEnvironment e, INotyfService notyf, IEmailSender emailSender)
        {
            _context = context;
            _he = e;
            _notyf = notyf;
            _emailSender = emailSender;
        }

        //Post
        public IActionResult AddCategory(string NewName)
        {
            if (NewName == null)
            {
                _notyf.Error("Não é possivel adicionar uma categoria sem nome");
                return PartialView("CategoryListPartial", _context.Categoria);
            }

            Categorium cat = new Categorium();
            cat.Nome = NewName;
            _context.Categoria.Add(cat);
            _context.SaveChanges();
            
            return PartialView("CategoryListPartial", _context.Categoria);
        }

        //GET
        public IActionResult EditCategory(int Id)
        {
            Categorium c = _context.Categoria.Find(Id);        
            return PartialView("EditCategory", c);
        }

        //Post
        [HttpPost]
        public string EditCategory(int id, Categorium c)
        {
            _context.Update(c);
            _context.SaveChanges();
            return c.Nome;
        }

        //Post
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            Categorium c = _context.Categoria.Find(Id);      
            
            foreach(CategoriasFilme film in _context.CategoriasFilmes)
            {
                if(film.IdCategoria == Id)
                {
                    _notyf.Error("Impossivel eliminar enquanto existirem filmes desta categoria");
                    return PartialView("CategoryListPartial", _context.Categoria);
                }
            }

            foreach(CategoriasFavorita cat in _context.CategoriasFavoritas)
            {
                if (cat.IdCategoria == Id)
                    _context.Remove(cat);
            }

            _context.Remove(c);
            await _context.SaveChangesAsync();

            return PartialView("CategoryListPartial", _context.Categoria);
        }

        //Get
        public async Task<IActionResult> CategoryList()
        {
            var cinemaXContext = _context.Categoria;

            return View(await cinemaXContext.ToListAsync());
        }

        //Post
        public IActionResult AddGroup(string NewName)
        {
            if (NewName == null)
            {
                _notyf.Error("Não é possivel adicionar um Grupo sem nome");
                ViewBag.Permissoes = _context.Permissoes;
                ViewBag.ListaPerm = _context.ListaPermissoes;
                return PartialView("PermissoesPartial", _context.GrupoPermissoes);
            }

            GrupoPermisso grupo = new GrupoPermisso();
            grupo.NomeGrupo = NewName;
            _context.GrupoPermissoes.Add(grupo);
            _context.SaveChanges();

            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;
            return PartialView("PermissoesPartial", _context.GrupoPermissoes);
        }

        //Post
        public async Task<IActionResult> DeleteUser(int Id)
        {
            Perfil P = _context.Perfils.Find(Id);
            Utilizador U = _context.Utilizadors.Find(Id);
            _context.Perfils.Remove(P);
            _context.Utilizadors.Remove(U);
            await _context.SaveChangesAsync();          
            
            return PartialView("UserPartial", _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation));
        }

        //Post
        public async Task<IActionResult> DeleteGroup(int Id)
        {
            if (_context.GrupoPermissoes.FirstOrDefault(l => l.IdGrupo == Id) != null)
            {
                _notyf.Error("Impossivel eliminar enquanto o grupo contiver Utilizadores");
            }
            else
            {
                foreach(ListaPermisso list in _context.ListaPermissoes)
                {
                    if (list.IdGrupo == Id)
                        _context.Remove(list);
                }

                GrupoPermisso G = _context.GrupoPermissoes.FirstOrDefault(x => x.IdGrupo == Id);
                _context.GrupoPermissoes.Remove(G);
                await _context.SaveChangesAsync();
            }

            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;
            return PartialView("PermissoesPartial", _context.GrupoPermissoes);
        }

            //Get
            public async Task<IActionResult> Permissoes()
        {
            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;

            var cinemaXContext = _context.GrupoPermissoes;
           
            return View(await cinemaXContext.ToListAsync());
        }

        //Post
        [HttpPost]
        public async Task<IActionResult> SaveGroups(int id, int []perms)
        {
            GrupoPermisso grupo = _context.GrupoPermissoes.Find(id);

            foreach(ListaPermisso perm in _context.ListaPermissoes)
            {
                if (perm.IdGrupo == grupo.IdGrupo)
                    _context.ListaPermissoes.Remove(perm);
            }

            foreach(int perm in perms)
            {
                ListaPermisso lista = new ListaPermisso() ;
                lista.IdGrupo = grupo.IdGrupo;
                lista.IdPermissao = perm;
                _context.ListaPermissoes.Add(lista);
            }

            await _context.SaveChangesAsync();
            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;
            _notyf.Success("Guardado com sucesso");
            return PartialView("PermissoesPartial", _context.GrupoPermissoes);
        }

        // GET: Utilizadors
        public async Task<IActionResult> UserList()
        {
            var cinemaXContext = _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation);
            return View(await cinemaXContext.ToListAsync());
        }

        public IActionResult AddUser()
        {
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo");
            return View();
        }

        //Get
        public IActionResult EditUser(int Id)
        {
            Utilizador a = _context.Utilizadors.Include(x=> x.IdGrupoNavigation).SingleOrDefault(x => x.IdUtilizador == Id);
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo");
            return PartialView("EditUserRole", a);
        }

        [HttpPost]
        public string EditUser(int id, Utilizador p)
        {
            _context.Update(p);
            _context.SaveChanges();
            p.IdGrupoNavigation = _context.GrupoPermissoes.FirstOrDefault(g => g.IdGrupo == p.IdGrupo);
            return p.IdGrupoNavigation.NomeGrupo;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([Bind("UserName,IdGrupo")] Utilizador utilizador, [Bind("Email")] Perfil perfil)
        {
            if (_context.Utilizadors.Where(u => u.UserName == utilizador.UserName).Count() != 0)
                ModelState.AddModelError("UserName", "Nome de utilizador ja existente");

            if (_context.Perfils.Where(u => u.Email == perfil.Email).Count() != 0)
                ModelState.AddModelError("Perfil.Email", "email ja registrado");

            
                utilizador.UserPassWord = "temp";
                utilizador.ActivationCode = GenerateNewCode(25);

                _context.Add(utilizador);

                await _context.SaveChangesAsync();

                perfil.IdUtilizador = utilizador.IdUtilizador;
                perfil.IdUtilizadorNavigation = utilizador;
                perfil.DataNascimento = DateTime.Now;
                perfil.Nome = "temp";
                perfil.Telemovel = 928888888;
                _context.Add(perfil);

                await _context.SaveChangesAsync();

                EnviaEmail(perfil.Email, utilizador.ActivationCode, utilizador.UserName);

                return RedirectToAction(nameof(UserList));                   
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

       
        //POST: BackOffice/AddRoom
        [HttpPost]
        public async Task<IActionResult> AddRoom(int? number)
        {
            if (number == null)
            {
                _notyf.Error("Não é possivel adicionar uma sala sem capacidade");
                return PartialView("RoomsPartial", _context.Salas);
            }
            Sala sala = new Sala();
            sala.Capacidade = (int)number;
            sala.DataAdicionada = DateTime.Now.Date;
            sala.IdCreationUser = (int)HttpContext.Session.GetInt32("IdUtilizador");
            _context.Add(sala);
            await _context.SaveChangesAsync();

            return PartialView("RoomsPartial", _context.Salas);
        }

        // GET: BackOffice/EditRoom/?
        public IActionResult EditRoom(int? id)
        {
            Sala sala = _context.Salas.Find(id);
            return PartialView("EditRoom", sala);
        }

        // POST: BackOffice/EditRoom
        [HttpPost]
        public async Task<int> EditRoom(int id, Sala sala)
        {
            _context.Update(sala);
            await _context.SaveChangesAsync();

            return sala.Capacidade;
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

            if (_context.Sessaos.FirstOrDefault(s => s.Numero == id) != null)
            {
                _notyf.Error("Impossivel eliminar sala se esta ainda contiver sessoes");
                return PartialView("RoomsPartial", _context.Salas);
            }

            var sala = await _context.Salas.FindAsync(id);
            
            if (sala == null)
            {
                return NotFound();
            }

            _context.Remove(sala);
            await _context.SaveChangesAsync();

            return PartialView("RoomsPartial", _context.Salas);
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

        //Function
        public static string GenerateNewCode(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % availableCharacters.Length;
                identifier[idx] = availableCharacters[pos];
            }

            return new string(identifier);
        }

        //Funciton
        public void EnviaEmail(string email, string code, string user)
        {
            int? id = HttpContext.Session.GetInt32("IdUtilizador");


            string Destino, Assunto, Mensagem;

            string Url = "https://localhost:44341/Utilizadors/AccountActivate/" + code;

            Destino = email;
            Assunto = "Foi convidado a se registrar no webiste CinemaX";
            Mensagem = "<h1>Bem vindo ao CinemaX</h1> <br/>A sua conta foi criada por um administrador, o seu nome de utilizador é"+ user +", acabe o registro no link a baixo" +
            "<br/><a href=\"" + Url + "\">Clique aqui para completar a sua conta</a>";
            TesteEnvioEmail(Destino, Assunto, Mensagem).GetAwaiter();
        }

        //Funciton
        public async Task TesteEnvioEmail(string email, string assunto, string mensagem)
        {
            try
            {
                //email destino, assunto do email, mensagem a enviar
                await _emailSender.SendEmailAsync(email, assunto, mensagem);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
