using AspNetCoreHero.ToastNotification.Abstractions;
using CinemaX.Data;
using CinemaX.Models;
using CinemaX.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CinemaXContext _context;
        private readonly INotyfService _notyf;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, CinemaXContext context, INotyfService notyf, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _notyf = notyf;
            _emailSender = emailSender;
        }

        //Post
        //Comprar bilhetes
        [HttpPost]
        public async Task<IActionResult> Comprar(int id, int numero)
        {
            Sessao sessao = _context.Sessaos.Find(id);
            int vagas = sessao.Vagas;

            if(numero > vagas)
            {
                _notyf.Error("Não exite vagas sufecientes para a quantidade de bilhetes");
                return Redirect("/Home/ComprarBilhete/"+id.ToString());
            }

            //ciclo para comprar o numero de bilhetes escolhidos pelo utilizador
            for (int i = numero; i > 0; i--)
            {
                Bilhete bilhete = new Bilhete();
                Utilizador utilizador = _context.Utilizadors.Find(HttpContext.Session.GetInt32("IdUtilizador"));

                bilhete.IdSessao = sessao.IdSessao;
                bilhete.IdSessaoNavigation = sessao;
                bilhete.IdUtilizador = utilizador.IdUtilizador;
                bilhete.IdUtilizadorNavigation = utilizador;


                _context.Add(bilhete);
                sessao.Vagas = sessao.Vagas - 1;
                _context.Update(sessao);
                await _context.SaveChangesAsync();

                sessao.IdSalaNavigation = _context.Salas.Find(sessao.IdSala);

                //Envia email ao utilizador com o bilhete
                EnviaEmail(_context.Perfils.Find(utilizador.IdUtilizador).Email, sessao);
            }

            return RedirectToAction(nameof(Index));
        }

        //Get
        //Comprar bilhete
        public IActionResult ComprarBilhete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Sessao sessao = _context.Sessaos.Find(id);

            if (HttpContext.Session.GetInt32("IdUtilizador") == null)
            {
                _notyf.Error("Precisa de iniciar sessao para comprar bilhetes");
                return Redirect("/Home/Details/" + sessao.IdFilme.ToString());
            }
           
            if(sessao == null)
            {
                return NotFound();
            }           

            sessao.IdFilmeNavigation = _context.Filmes.Find(sessao.IdFilme);
            sessao.IdSalaNavigation = _context.Salas.Find(sessao.IdSala);

            return View(sessao);
        }

        //Get
        //Cartaz
        public IActionResult Index()
        {
            //Carrega uma lista com os ultimos 4 filmes
            List<Filme> cartaz = _context.Filmes.Skip(Math.Max(0, _context.Filmes.Count() - 4)).ToList();

            cartaz.Reverse();

            return View(cartaz);
        }

        //Get
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Get
        //Detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Filmes.Find(id) == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.Include(u => u.CategoriasFilmes).Include(u=>u.Sessaos)
                .FirstOrDefaultAsync(m => m.IdFilme == id);

            //Adiciona as categorias aos filmes
            foreach (var cat in filme.CategoriasFilmes)
            {
                filme.CategoriasFilmes.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdFilme == cat.IdFilme).IdCategoriaNavigation = _context.Categoria.Find(cat.IdCategoria);
            }
            
            //Especifica o caminho para onde voltar dependendo de onde foi redirecionado
            if (Request.Headers["Referer"].ToString().Contains("/Home/Filmes"))
            {
                ViewBag.Voltar = "Filmes";
            }
            else
            {
                ViewBag.Voltar = "Index";
            }

            return View(filme);
        }

        //Get
        //Pagina de procurar filmes
        public async Task<IActionResult> Filmes()
        {
            #region Data preparation
            //É criada uma lista com os nomes dos filmes e realizadores para conseguirem ser autopreenchidos atravez de ajax
            List<string> NomeFilmes = new List<string>();
            List<string> NomeRealizador = new List<string>();

            //São preenchidas as listas com os dados
            foreach(var filme in _context.Filmes)
            {
                NomeFilmes.Add(filme.Nome);
                if(!NomeRealizador.Contains(filme.Realizador))
                    if(!filme.Realizador.Contains(","))
                        NomeRealizador.Add(filme.Realizador);
                    else
                    {
                        //Se o filme continver mais doque um realizador estes são divididos apartir das virgulas
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

        //Get
        //Procura um filme na base de dados
        public IActionResult Procurar(string NomeFilme, string NomeRealizador, int[] IdCategorias, DateTime? DataInicio, DateTime? DataFim)
        {
            List<Filme> filmes = new List<Filme>();

            //Verifica todas as possiveis conbinações de dados introduzidos depois de escolhida a certa enche um ViewBag com os filmes que pertençam ás regras introduzidas
            if (NomeFilme == null && NomeRealizador == null && IdCategorias.Length == 0 && (DataInicio == null || DataFim == null))
            {
                ViewBag.Filmes = _context.Filmes;
            }

            else if(NomeRealizador == null && IdCategorias.Length == 0 && (DataInicio == null || DataFim == null))
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeFilme == null && IdCategorias.Length == 0 && (DataInicio == null || DataFim == null))
            {
                foreach(var film in _context.Filmes)
                {
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }
            else if (NomeFilme == null && NomeRealizador == null && (DataInicio == null || DataFim == null))
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f=>f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if(filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (NomeFilme == null && NomeRealizador == null && IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Data >= DataInicio && film.Data <= DataFim)
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(IdCategorias.Length == 0 && (DataInicio == null || DataFim == null))
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeFilme == null && (DataInicio == null || DataFim == null))
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeRealizador == null && (DataInicio == null || DataFim == null))
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (NomeRealizador == null && NomeFilme == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if ((film.Data >= DataInicio && film.Data <= DataFim) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (NomeRealizador == null && IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (NomeFilme == null && IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(DataInicio == null || DataFim == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if(NomeFilme == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (NomeRealizador == null)
            {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else if (IdCategorias.Length == 0)
            {
                foreach (var film in _context.Filmes)
                {                   
                    if (film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            else {
                foreach (var film in _context.Filmes)
                {
                    var filmee = _context.CategoriasFilmes.FirstOrDefault(f => f.IdFilme == film.IdFilme && IdCategorias.Contains(f.IdCategoria));
                    if (film.Realizador.Contains(NomeRealizador, StringComparison.InvariantCultureIgnoreCase) && film.Nome.Contains(NomeFilme, StringComparison.InvariantCultureIgnoreCase) && (film.Data >= DataInicio && film.Data <= DataFim) && filmee != null && !filmes.Contains(film))
                        filmes.Add(film);
                }

                ViewBag.Filmes = filmes;
            }

            return PartialView("Filme");
        }

        //Envia email
        public void EnviaEmail(string email, Sessao session)
        {
            int? id = HttpContext.Session.GetInt32("IdUtilizador");

            session.IdFilmeNavigation = _context.Filmes.Find(session.IdFilme);

            string Destino, Assunto, Mensagem;
            Destino = email;
            Assunto = "CinemaX - bilhete para " + session.IdFilmeNavigation.Nome;
            Mensagem = "<h1>Bilhete:</h1>" +
                "Filme: " + session.IdFilmeNavigation.Nome + "<br/>" +
                "Sala: " + session.IdSalaNavigation.Numero + "<br/>" +
                "Data: " + session.Data + "<br/>";             
            TesteEnvioEmail(Destino, Assunto, Mensagem).GetAwaiter();
        }

        //Envia email
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
