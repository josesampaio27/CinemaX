using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaX.Data;
using CinemaX.Models;
using Microsoft.AspNetCore.Http;
using CinemaX.Services;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using AspNetCoreHero.ToastNotification.Abstractions;
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CinemaX.Controllers
{
    public class UtilizadorsController : Controller
    {
        private readonly CinemaXContext _context;
        private readonly IEmailSender _emailSender;
        static readonly char[] availableCharacters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
  };

       private readonly INotyfService _notyf;

        public UtilizadorsController(CinemaXContext context, IEmailSender emailSender, INotyfService notyf)
        {
            _context = context;
            _emailSender = emailSender;
            _notyf = notyf;
        }

        [HttpGet]
        [Route("Utilizadors/AccountActivate/{*code}")]
        public IActionResult AccountActivate(string code)
        {
            Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.ActivationCode == code);           
            if (user != null)
            {
                user.Perfil = _context.Perfils.Find(user.IdUtilizador);
                user.Perfil.DataNascimento = DateTime.Now;
                user.Perfil.Nome = null;
                user.Perfil.Telemovel = 9;

                return View(user);
            }

            return RedirectToAction("ActivationError", "Utilizadors");

        }

        [HttpPost]
        public async Task<IActionResult> AccountActivate([Bind("UserPassWord,IdUtilizador,IdGrupo,UserName")] Utilizador utilizador, [Bind("Nome,DataNascimento,Telemovel,Email")] Perfil perfil)
        {
            
            if (ModelState.IsValid)
            {
                utilizador.ActivationCode = "Activated";
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;

                _context.Update(utilizador);
                await _context.SaveChangesAsync();

                perfil.IdUtilizador = utilizador.IdUtilizador;
                _context.Update(perfil);
                await _context.SaveChangesAsync();             
                return RedirectToAction("Index","Home");
            }
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo", utilizador.IdGrupo);
            return View(utilizador);
        }

        // GET: Utilizadors/Register
        public IActionResult Register()
        {           
            return View();
        }

        public IActionResult Login()
        {
            if(Request.Headers["Referer"].ToString().Contains("/Utilizadors/Activate"))
                _notyf.Success("Utilizador confirmado com sucesso");

            return View();
        }

        public async Task<IActionResult> HistoricoBilhetes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id != HttpContext.Session.GetInt32("IdUtilizador"))
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            var Perfil = await _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation).FirstOrDefaultAsync(m => m.IdUtilizador == id);

            if (_context.Bilhetes.FirstOrDefault(b => b.IdUtilizador == Perfil.IdUtilizador) != null)
            {
                List<Bilhete> list = new List<Bilhete>();

                foreach (Bilhete bilhete in _context.Bilhetes.Include(b => b.IdSessaoNavigation.IdFilmeNavigation).Include(b => b.IdSessaoNavigation.NumeroNavigation))
                {
                    if (bilhete.IdUtilizador == Perfil.IdUtilizador)
                    {
                        list.Add(bilhete);
                        ViewData[bilhete.NumBilhete.ToString()] = GenerateQRCode("Sessao:" + bilhete.IdSessao + ";NumBilhete:" + bilhete.NumBilhete + ";");
                    }
                }

                list.Reverse();

                ViewBag.Bilhetes = list;
            }

            return View(Perfil);
        }

        public async Task<IActionResult> Perfil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if(id != HttpContext.Session.GetInt32("IdUtilizador"))
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            

            var Perfil = await _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation).FirstOrDefaultAsync(m => m.IdUtilizador == id);

            foreach (var cat in _context.CategoriasFavoritas)
            {
                if(_context.CategoriasFavoritas.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id) != null)
                    Perfil.IdUtilizadorNavigation.CategoriasFavorita.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
            }

            if (Perfil == null)
            {
                return NotFound();
            }

            if(_context.Bilhetes.FirstOrDefault(b=> b.IdUtilizador == Perfil.IdUtilizador) != null)
            {
                List<Bilhete> list = new List<Bilhete>();

                foreach(Bilhete bilhete in _context.Bilhetes.Include(b => b.IdSessaoNavigation.IdFilmeNavigation).Include(b=>b.IdSessaoNavigation.NumeroNavigation))
                {
                    if (bilhete.IdUtilizador == Perfil.IdUtilizador)
                    {
                        list.Add(bilhete);
                        ViewData[bilhete.NumBilhete.ToString()] = GenerateQRCode("Sessao:"+bilhete.IdSessao+";NumBilhete:"+bilhete.NumBilhete+";");
                    }
                }


                list.Reverse();

                if (list.Count > 5)
                    ViewBag.Bilhete = list.Take(5);
                else
                    ViewBag.Bilhete = list;

            }

            return View(Perfil);
        }

        public string GenerateQRCode(string QRString)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRString, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap bitMap = qrCode.GetGraphic(20);
                bitMap.Save(ms, ImageFormat.Png);
                return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }

        public IActionResult EditarCategoriasFavoritas()
        {
            if (HttpContext.Session.GetInt32("IdUtilizador") == null)
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            int id = (int)HttpContext.Session.GetInt32("IdUtilizador");

            var Perfil = _context.Perfils.Include(p=> p.IdUtilizadorNavigation).FirstOrDefault(m => m.IdUtilizador == id);

            foreach (CategoriasFavorita cat in _context.CategoriasFavoritas)
            {
                if (_context.CategoriasFavoritas.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id) != null)
                {
                    cat.IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
                    Perfil.IdUtilizadorNavigation.CategoriasFavorita.Add(cat);                        
                }
            }

            ViewBag.Categorias = _context.Categoria;

            if (Perfil == null)
            {
                return NotFound();
            }

            return View(Perfil);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCategoriasFavoritas(int[] IdCategorias)
        {
            int id = (int)HttpContext.Session.GetInt32("IdUtilizador");

            foreach (CategoriasFavorita cat in _context.CategoriasFavoritas)
            {
                if (cat.IdUtilizador == id)
                    _context.Remove(cat);
            }

            foreach (int IdCat in IdCategorias)
            {               

                CategoriasFavorita aux = new CategoriasFavorita();
                aux.IdCategoria = IdCat;
                aux.IdUtilizador = id;

                Categorium cat = _context.Categoria.FirstOrDefault(c => c.IdCategoria == aux.IdCategoria);
                aux.IdCategoriaNavigation = cat;

                Utilizador ut = _context.Utilizadors.FirstOrDefault(u => u.IdUtilizador == aux.IdUtilizador);
                aux.IdUtilizadorNavigation = ut;
             
                _context.Add(aux);              
            }

            await _context.SaveChangesAsync();          
            return Redirect("Perfil/"+id.ToString());
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ActivationError()
        {
            return View();
        }

        public IActionResult ActivationWarning()
        {
            return View();
        }

        [Route("Utilizadors/Activate/{*code}")]
        public async Task<IActionResult> Activate(string code)
        {
            Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.ActivationCode == code);

            if (user != null)
            {
                user.ActivationCode = "Activated";

                _context.Update(user);

                await _context.SaveChangesAsync();
                               
                return View();
            }

            return RedirectToAction("ActivationError", "Utilizadors");

        }

        // POST: Utilizadors/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,UserPassWord,IdUtilizador,IdGrupo")] Utilizador utilizador, [Bind("Nome,Email,DataNascimento,Telemovel")] Perfil perfil)
        {
            if (_context.Utilizadors.Where(u => u.UserName == utilizador.UserName).Count() != 0)
                ModelState.AddModelError("UserName", "Nome de utilizador ja existente");

            if (_context.Perfils.Where(u => u.Email == perfil.Email).Count() != 0)
                ModelState.AddModelError("Perfil.Email", "email ja registrado");

            if (ModelState.IsValid)
            {
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;
                utilizador.IdGrupo = 1;
                utilizador.ActivationCode = GenerateNewCode(25);

                _context.Add(utilizador);

                await _context.SaveChangesAsync();
                perfil.IdUtilizador = utilizador.IdUtilizador;
                perfil.IdUtilizadorNavigation = utilizador;

                _context.Add(perfil);
                await _context.SaveChangesAsync();
                EnviaEmail(perfil.Email,utilizador.ActivationCode);
                return RedirectToAction(nameof(ActivationWarning));
            }
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo", utilizador.IdGrupo);
            return View(utilizador);
        }

        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("UserName,UserPassWord")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;

                Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.UserName == utilizador.UserName && u.UserPassWord == utilizador.UserPassWord);

                if (user == null){
                    ModelState.AddModelError("UserName", "Utilizador ou password erradas!");
                    return View(utilizador);
                }

                if(user.ActivationCode != "Activated")
                {
                    ModelState.AddModelError("UserName", "Utilizador não confirmado");
                    return View(utilizador);
                }

                HttpContext.Session.SetInt32("IdUtilizador", user.IdUtilizador);
                HttpContext.Session.SetString("NomeUtilizador", user.UserName);

                string permissoes = null;
                
                foreach (var lista in _context.ListaPermissoes)
                {
                    if (lista.IdGrupo == user.IdGrupo)
                    {
                        permissoes += lista.IdPermissao.ToString();
                    }
                }

                if(permissoes != null)
                HttpContext.Session.SetString("Permissoes", permissoes);


                return RedirectToAction("index","Home");
            }           
            return View(utilizador);
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadors.Any(e => e.IdUtilizador == id);
        }

        public void EnviaEmail(string email, string code)
        {
            int? id = HttpContext.Session.GetInt32("IdUtilizador");


            string Destino, Assunto, Mensagem;

            string Url = "https://localhost:44341/Utilizadors/Activate/"+code;

            Destino = email;
            Assunto = "Confirme o registo no website CinemaX";
            Mensagem = "<h1>Bem vindo ao CinemaX</h1> <br/>A sua conta foi criada com sucesso, confirme a sua conta para poder começar a usufruir do nosso website" +
            "<br/><a href=\""+Url+"\">Clique aqui para confirmar a sua conta</a>";
            TesteEnvioEmail(Destino, Assunto, Mensagem).GetAwaiter();                                    
        }
        
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



    }
}
