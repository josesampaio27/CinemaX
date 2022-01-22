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
        [HttpPost]
        public IActionResult AddCategory(string NewName)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(7))
                return RedirectToAction(nameof(PermissionDenied));

            if (NewName == null)
            {
                _notyf.Error("Não é possivel adicionar uma categoria sem nome");
                return PartialView("CategoryListPartial", _context.Categoria);
            }

            //verifica o tamanho maximo de uma categoria
            if(NewName.Length > 30)
            {
                _notyf.Error("O tamanho maximo para uma categoria é de 30 caracteres");
                return PartialView("CategoryListPartial", _context.Categoria);
            }

            Categorium cat = new Categorium();
            cat.Nome = NewName;
            _context.Categoria.Add(cat);
            _context.SaveChanges();
            
            return PartialView("CategoryListPartial", _context.Categoria);
        }

        //GET
        //Edita uma categoria
        public IActionResult EditCategory(int Id)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(7))
                return RedirectToAction(nameof(PermissionDenied));

            Categorium c = _context.Categoria.Find(Id);        
            return PartialView("EditCategory", c);
        }

        //Post
        //Edita uma categoria
        [HttpPost]
        public string EditCategory(int id, Categorium c)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(7))
                return null;

            _context.Update(c);
            _context.SaveChanges();
            return c.Nome;
        }

        //Post
        //Apaga uma categoria
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(7))
                return RedirectToAction(nameof(PermissionDenied));

            Categorium c = _context.Categoria.Find(Id);      
            
            //Se existir filmes com essa categoria notifica que é impossivel eliminar
            foreach(CategoriasFilme film in _context.CategoriasFilmes)
            {
                if(film.IdCategoria == Id)
                {
                    _notyf.Error("Impossivel eliminar enquanto existirem filmes desta categoria");
                    return PartialView("CategoryListPartial", _context.Categoria);
                }
            }

            //Rtira as categorias favoritas dos utilizadores que continham esta categoria
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
        //Lista de categorias
        public async Task<IActionResult> CategoryList()
        {
            //Verifica se utilizador tem permissão
            if (!Perm(7))
                return RedirectToAction(nameof(PermissionDenied));

            var cinemaXContext = _context.Categoria;

            return View(await cinemaXContext.ToListAsync());
        }

        //Post
        //Adicionar grupo de utilizadores
        [HttpPost]
        public IActionResult AddGroup(string NewName)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(8))
                return RedirectToAction(nameof(PermissionDenied));

            if (NewName == null)
            {
                _notyf.Error("Não é possivel adicionar um Grupo sem nome");
                ViewBag.Permissoes = _context.Permissoes;
                ViewBag.ListaPerm = _context.ListaPermissoes;
                return PartialView("PermissoesPartial", _context.GrupoPermissoes);
            }

            //verifica o tamanho do nome inserido
            if (NewName.Length > 30)
            {
                _notyf.Error("O tamanho maximo para um Grupo é de 30 caracteres");
                return PartialView("CategoryListPartial", _context.Categoria);
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
        //Apaga utilizador
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return RedirectToAction(nameof(PermissionDenied));

            Perfil P = _context.Perfils.Find(Id);
            Utilizador U = _context.Utilizadors.Find(Id);
            _context.Perfils.Remove(P);
            _context.Utilizadors.Remove(U);
            await _context.SaveChangesAsync();          
            
            return PartialView("UserPartial", _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation));
        }

        //Post
        //Aapaga grupo de permissoes
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int Id)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(8))
                return RedirectToAction(nameof(PermissionDenied));

            //Se algum utilizador pertencer a este grupo notifica que é impossivel apagar
            if (_context.Utilizadors.FirstOrDefault(l => l.IdGrupo == Id) != null)
            {
                _notyf.Error("Impossivel eliminar enquanto o grupo contiver Utilizadores");
            }
            else
            {
                //Retira as permissões do grupo
                foreach(ListaPermisso list in _context.ListaPermissoes)
                {
                    if (list.IdGrupo == Id)
                        _context.Remove(list);
                }

                GrupoPermisso G = _context.GrupoPermissoes.Find(Id);
                _context.GrupoPermissoes.Remove(G);
                await _context.SaveChangesAsync();
            }

            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;
            return PartialView("PermissoesPartial", _context.GrupoPermissoes);
        }

        //Get
        //Lista de grupos e permissões
        public async Task<IActionResult> Permissoes()
        {
            //Verifica se utilizador tem permissão
            if (!Perm(8))
                return RedirectToAction(nameof(PermissionDenied));

            ViewBag.Permissoes = _context.Permissoes;
            ViewBag.ListaPerm = _context.ListaPermissoes;

            var cinemaXContext = _context.GrupoPermissoes;
           
            return View(await cinemaXContext.ToListAsync());
        }

        //Post
        //Guarda as alterações feitas ás permissões do grupo
        [HttpPost]
        public async Task<IActionResult> SaveGroups(int id, int []perms)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(8))
                return RedirectToAction(nameof(PermissionDenied));

            GrupoPermisso grupo = _context.GrupoPermissoes.Find(id);

            //Retira todas as permissões do grupo
            foreach(ListaPermisso perm in _context.ListaPermissoes)
            {
                if (perm.IdGrupo == grupo.IdGrupo)
                    _context.ListaPermissoes.Remove(perm);
            }

            //Asiciona as permissões selecionadas pelo utilizador
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
            //notificação
            _notyf.Success("Guardado com sucesso");
            return PartialView("PermissoesPartial", _context.GrupoPermissoes);
        }

        // GET: Utilizadors
        //Lista de todos os utilizadores
        public async Task<IActionResult> UserList()
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return RedirectToAction(nameof(PermissionDenied));

            var cinemaXContext = _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation);
            return View(await cinemaXContext.ToListAsync());
        }

        //Get
        //Convidar utilizador
        public IActionResult AddUser()
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return RedirectToAction(nameof(PermissionDenied));

            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo");
            return View();
        }

        //Get
        //Editar utilizador
        public IActionResult EditUser(int Id)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return RedirectToAction(nameof(PermissionDenied));

            Utilizador a = _context.Utilizadors.Include(x=> x.IdGrupoNavigation).SingleOrDefault(x => x.IdUtilizador == Id);
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo");
            return PartialView("EditUserRole", a);
        }

        //Post
        //Editar utilizador
        [HttpPost]
        public string EditUser(int id, Utilizador p)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return null;

            _context.Update(p);
            _context.SaveChanges();
            p.IdGrupoNavigation = _context.GrupoPermissoes.FirstOrDefault(g => g.IdGrupo == p.IdGrupo);
            return p.IdGrupoNavigation.NomeGrupo;
        }

        //Post
        //Adicionar utilizador
        [HttpPost]
        public async Task<IActionResult> AddUser([Bind("UserName,IdGrupo")] Utilizador utilizador, [Bind("Email")] Perfil perfil)
        {
            //Verifica se utilizador tem permissão
            if (!Perm(3))
                return RedirectToAction(nameof(PermissionDenied));

            //Verifica se ja exite o username
            if (_context.Utilizadors.Where(u => u.UserName == utilizador.UserName).Count() != 0)
                ModelState.AddModelError("UserName", "Nome de utilizador ja existente");

            //verifica se ja exite esse email
            if (_context.Perfils.Where(u => u.Email == perfil.Email).Count() != 0)
                ModelState.AddModelError("Perfil.Email", "email ja registrado");

            
                utilizador.UserPassWord = "temp";
                utilizador.ActivationCode = GenerateNewCode(25);

            ModelState.Remove("UserPassWord");
            ModelState.Remove("ActivationCode");          
            ModelState.Remove("Perfil.DataNascimento");
            ModelState.Remove("Perfil.Nome");
            ModelState.Remove("Perfil.Telemovel");


            if (ModelState.IsValid)
            {
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

            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo");
            return View(utilizador);
        }

        // GET: BackOffice/FilmList
        public async Task<IActionResult> FilmList()
        {
            if (!Perm(2))
                return RedirectToAction(nameof(PermissionDenied));
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: BackOffice/Rooms
        public async Task<IActionResult> Rooms()
        {
            if (!Perm(4))
                return RedirectToAction(nameof(PermissionDenied));
            return View(await _context.Salas.ToListAsync());
        }

        // GET: BackOffice/AddMovie
        public IActionResult AddMovie()
        {
            if(!Perm(2))
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
            if (!Perm(5))
                return RedirectToAction(nameof(PermissionDenied));

            foreach (Sessao sessao in _context.Sessaos) {
                ViewData[sessao.IdSessao.ToString()] = _context.Bilhetes.Where(b => b.IdSessao == sessao.IdSessao).Count().ToString();
            }

            return View(await _context.Sessaos.Include(f => f.IdFilmeNavigation).Include(f=> f.IdSalaNavigation).OrderByDescending(x=>x.Data).ToListAsync());
        }

        // GET: BackOffice/AddSession
        public IActionResult AddSession()
        {
            if (!Perm(5))
                return RedirectToAction(nameof(PermissionDenied));


            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "IdSala","Numero");           
            return View();
        }

        //POST: BackOffice/AddSession
        [HttpPost]
        public async Task<IActionResult> AddSession([Bind("IdFilme,IdSala,Data,Preço_string")] Sessao sessao)
        {
            if (!Perm(5))
                return RedirectToAction(nameof(PermissionDenied));

            if (sessao.Data < DateTime.Now)
            {
                ModelState.AddModelError("Data", "Data invalida");
            }

            if (ModelState.IsValid)
            {
                sessao.Preço = decimal.Parse(sessao.Preço_string, CultureInfo.InvariantCulture);

                Filme filme;
                Sala sala;

                if ( (filme = _context.Filmes.FirstOrDefault(f=>f.IdFilme == sessao.IdFilme)) == null)
                    return NotFound();

                if ((sala = _context.Salas.Find(sessao.IdSala)) == null)
                    return NotFound();

                sessao.IdFilmeNavigation = filme;
                sessao.IdSalaNavigation = sala;

                sessao.Vagas = sala.Capacidade;

                _context.Add(sessao);
                await _context.SaveChangesAsync();
                _notyf.Success("Sessao adicionada com sucesso");
                return RedirectToAction(nameof(SessionList));
            }


            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "Numero", "Numero");
            return View(sessao);
        }

        // GET: BackOffice/EditSession
        public IActionResult EditSession(int? id)
        {
            if (!Perm(5))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }           

            if (_context.Bilhetes.FirstOrDefault(b => b.IdSessao == id) != null)
            {
                _notyf.Error("Impossivel editar uma sessão com bilhetes ja vendidos");
                return RedirectToAction("SessionList");
            }

            var sessao = _context.Sessaos.FirstOrDefault(s => s.IdSessao == id);

            ViewData["IdFilme"] = new SelectList(_context.Filmes, "IdFilme", "Nome");
            ViewData["Numero"] = new SelectList(_context.Salas, "IdSala", "Numero");

            sessao.Preço_string = sessao.Preço.ToString().Replace(",",".");
            sessao.Preço_string = sessao.Preço_string.Remove(sessao.Preço_string.Length - 2, 2);

            return View(sessao);
        }

        //POST: BackOffice/EditSession
        [HttpPost]
        public async Task<IActionResult> EditSession(int id, [Bind("IdSessao,IdFilme,IdSala,Data,Preço_string")] Sessao sessao)
        {
            if (!Perm(5))
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

                if ((sala = _context.Salas.Find(sessao.IdSala)) == null)
                    return NotFound();

                sessao.IdFilmeNavigation = filme;
                sessao.IdSalaNavigation = sala;

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
        //public IActionResult DeleteSession(int? id)
        //{
        //    if (!Perm(5))
        //        return RedirectToAction(nameof(PermissionDenied));

        //    if(id == null)
        //        return NotFound();

        //    var sessao = _context.Sessaos.Find(id);

        //    sessao.IdFilmeNavigation = _context.Filmes.Find(sessao.IdFilme);
        //    sessao.IdSalaNavigation = _context.Salas.Find(sessao.IdSala);

        //    return View(sessao);
        //}

        // POST: BackOffice/DeleteSession 
        [HttpPost, ActionName("DeleteSession")]
        public async Task<IActionResult> DeleteSessionConfirmed(int id)
        {
            if (!Perm(5))
                return RedirectToAction(nameof(PermissionDenied));

            var sessao = await _context.Sessaos.FindAsync(id);

            foreach (Sessao sessaos in _context.Sessaos)
            {
                ViewData[sessaos.IdSessao.ToString()] = _context.Bilhetes.Where(b => b.IdSessao == sessaos.IdSessao).Count().ToString();
            }

            if (_context.Bilhetes.FirstOrDefault(b=>b.IdSessao == sessao.IdSessao) != null)
            {
                _notyf.Error("Impossivel eliminar sessão com bilhetes vendidos");

                return PartialView("SessionListPartial", await _context.Sessaos.Include(f => f.IdFilmeNavigation).Include(f => f.IdSalaNavigation).OrderByDescending(x => x.Data).ToListAsync());
            }

            _context.Sessaos.Remove(sessao);
            await _context.SaveChangesAsync();

            _notyf.Success("Sessao elimiada com sucesso");

            return PartialView("SessionListPartial", await _context.Sessaos.Include(f => f.IdFilmeNavigation).Include(f => f.IdSalaNavigation).OrderByDescending(x => x.Data).ToListAsync());
        }

       
        //POST: BackOffice/AddRoom
        [HttpPost]
        public async Task<IActionResult> AddRoom(int? number)
        {
            if (!Perm(4))
                return RedirectToAction(nameof(PermissionDenied));

            if (number == null)
            {
                _notyf.Error("Não é possivel adicionar uma sala sem capacidade");
                return PartialView("RoomsPartial", _context.Salas);
            }
            Sala sala = new Sala();
            sala.Capacidade = (int)number;
            sala.DataAdicionada = DateTime.Now.Date;
            sala.IdCreationUser = (int)HttpContext.Session.GetInt32("IdUtilizador");
            int numero = _context.Salas.Count();
            while (_context.Salas.FirstOrDefault(x=>x.Numero==numero) != null)
            {
                numero++;
            }
            sala.Numero = numero;
            _context.Add(sala);
            await _context.SaveChangesAsync();

            return PartialView("RoomsPartial", _context.Salas);
        }

        // GET: BackOffice/EditRoom/?
        public IActionResult EditRoom(int? id)
        {
            if (!Perm(4))
                return RedirectToAction(nameof(PermissionDenied));

            Sala sala = _context.Salas.Find(id);
            return PartialView("EditRoom", sala);
        }

        // POST: BackOffice/EditRoom
        [HttpPost]
        public async Task<int> EditRoom(int id, Sala sala)
        {
            if (!Perm(4))
                return -1;

            Sala salas = _context.Salas.Find(id);
            salas.Capacidade = sala.Capacidade;

            _context.Update(salas);
            await _context.SaveChangesAsync();

            return sala.Capacidade;
        }

        public IActionResult EditNumber(int? id)
        {
            if (!Perm(4))
                return RedirectToAction(nameof(PermissionDenied));

            Sala sala = _context.Salas.Find(id);
            return PartialView("EditNumber", sala);
        }

        // POST: BackOffice/EditNumero
        [HttpPost]
        public async Task<int> EditNumber(int id, Sala sala)
        {
            if (!Perm(4))
                return -1;

            Sala salas = _context.Salas.Find(id);       

            if (_context.Salas.FirstOrDefault(s=>s.Numero == sala.Numero) == null)
            {
                salas.Numero = sala.Numero;
                _context.Update(salas);
                await _context.SaveChangesAsync();
            }
            else
            {
                _notyf.Error("Ja exite uma sala com esse numero");
            }          

            return salas.Numero;
        }

        // GET: BackOffice/DeleteRoom/?
        public async Task<IActionResult> DeleteRoom(int? id)
        {
            if (!Perm(4))
                return RedirectToAction(nameof(PermissionDenied));

            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas.FindAsync(id);

            if (_context.Sessaos.Count() != 0 && _context.Sessaos.FirstOrDefault(s => s.IdSala == sala.IdSala) != null)
            {
                _notyf.Error("Impossivel eliminar sala se esta ainda contiver sessoes");
                return PartialView("RoomsPartial", _context.Salas);
            }
           
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
            if (!Perm(2))
                return RedirectToAction(nameof(PermissionDenied));

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

                EmailFavorites(filme.IdFilme);

                return RedirectToAction(nameof(FilmList));
            }

            ViewBag.Categorias = _context.Categoria;
            return View(filme);
        }

        // GET: BackOffice/EditMovie/?
        public IActionResult EditMovie(int? id)
        {
            if (!Perm(2))
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
            if (!Perm(2))
                return RedirectToAction(nameof(PermissionDenied));

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

        // GET: BackOffice/DeleteMovie/5
        public async Task<IActionResult> DeleteMovie(int? id)
        {
            if (!Perm(2))
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
            if (!Perm(2))
                return RedirectToAction(nameof(PermissionDenied));

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
        public void EnviaEmailCategoria(string email,string nomefilme)
        {
            int? id = HttpContext.Session.GetInt32("IdUtilizador");


            string Destino, Assunto, Mensagem;            

            Destino = email;
            Assunto = "Foi adicionado um filme com uma categoria que esta na sua lista de favoritos";
            Mensagem = "<h1>Foi adicionado um filme com uma categoria que esta na sua lista de favoritos</h1> <br/> O filme "+ nomefilme + " foi adicionado ao nosso website e contem" +
                " uma ou mais categorias que voce tem na lista de favoritos!";
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

        public void EmailFavorites(int id) {

            Filme filme = _context.Filmes.Find(id);

            List<int> emailcats = new List<int>();

            List<int> emailedusers = new List<int>();

            foreach(CategoriasFilme catlist in _context.CategoriasFilmes)
            {
                if (catlist.IdFilme == filme.IdFilme)
                    emailcats.Add(catlist.IdCategoria);
            }

            foreach(CategoriasFavorita catlist in _context.CategoriasFavoritas)
            {
                if (emailcats.Contains(catlist.IdCategoria) && !emailedusers.Contains(catlist.IdUtilizador))
                {
                    EnviaEmailCategoria(_context.Perfils.Find(catlist.IdUtilizador).Email,filme.Nome);
                    emailedusers.Add(catlist.IdUtilizador);
                }
            }
        }

    }
}
