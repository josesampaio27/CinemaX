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

        //GET
        //Finalização da criação de um utilizadoradicionado por um administrador
        [HttpGet]
        [Route("Utilizadors/AccountActivate/{*code}")]
        public IActionResult AccountActivate(string code)
        {
            //encontra o utilizador atravez do seu codigo unico
            Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.ActivationCode == code);           
            if (user != null)
            {
                //inicializa o perfil
                user.Perfil = _context.Perfils.Find(user.IdUtilizador);
                user.Perfil.DataNascimento = DateTime.Now;
                user.Perfil.Nome = null;
                user.Perfil.Telemovel = 9;

                return View(user);
            }

            return RedirectToAction("ActivationError", "Utilizadors");

        }

        //Get
        //Efetuação da mudança de password
        [HttpGet]
        [Route("Utilizadors/ConfirmChangePassword/{*code}")]
        public IActionResult ConfirmChangePassword(string code)
        {
            //encontra o utilizador atravez do seu codigo unico
            Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.ActivationCode == code);
            if(user != null)
            {
                return View(user);
            }

            return RedirectToAction("ActivationError", "Utilizadors");
        }

        //Post
        //Efetuação da mudança de password
        [HttpPost]
        public async Task<IActionResult> ConfirmChangePassword(Utilizador utilizador)
        {
            if(utilizador == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //Ativa novamente o utilizador
                utilizador.ActivationCode = "Activated";

                //criação da hash da password do utilizador
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;

                _context.Update(utilizador);
                await _context.SaveChangesAsync();

                //notificação
                _notyf.Success("Password alterada com sucesso");

                return RedirectToAction("Index", "Home");
            }

            return View(utilizador);
        }


        //Post
        //Finalização da criação de um utilizadoradicionado por um administrador
        [HttpPost]
        public async Task<IActionResult> AccountActivate([Bind("UserPassWord,IdUtilizador,IdGrupo,UserName")] Utilizador utilizador, [Bind("Nome,DataNascimento,Telemovel,Email")] Perfil perfil)
        {
            
            if (ModelState.IsValid)
            {
                //Ativa o utilizador
                utilizador.ActivationCode = "Activated";

                //criação da hash da password do utilizador
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

        //Get
        public IActionResult Login()
        {
            //Se o utilizador tiver sido redirecionado da ativação de conta ativa um aviso
            if(Request.Headers["Referer"].ToString().Contains("/Utilizadors/Activate"))
                _notyf.Success("Utilizador confirmado com sucesso");

            return View();
        }

        //Get
        //Todos os bilhetes ja comprados por um utilizador
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

            var Perfil = await _context.Perfils.FindAsync(id);

            //se o utilizador contiver bilhetes carrega uma lista com todos os seus bilhetes
            if (_context.Bilhetes.FirstOrDefault(b => b.IdUtilizador == Perfil.IdUtilizador) != null)
            {
                List<Bilhete> list = new List<Bilhete>();

                foreach (Bilhete bilhete in _context.Bilhetes.Include(b => b.IdSessaoNavigation.IdFilmeNavigation).Include(b => b.IdSessaoNavigation.IdSalaNavigation))
                {
                    if (bilhete.IdUtilizador == Perfil.IdUtilizador)
                    {
                        list.Add(bilhete);
                        //Gera o QrCode do bilhete
                        ViewData[bilhete.NumBilhete.ToString()] = GenerateQRCode("Sessao:" + bilhete.IdSessao + ";NumBilhete:" + bilhete.NumBilhete + ";");
                    }
                }

                //Troca a ordem da lista
                list.Reverse();

                ViewBag.Bilhetes = list;
            }

            return View(Perfil);
        }

        //Get
        //Perfil completo do utilizador
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
        
            var Perfil = await _context.Perfils.Include(u => u.IdUtilizadorNavigation).FirstOrDefaultAsync(m => m.IdUtilizador == id);

            if (Perfil == null)
            {
                return NotFound();
            }

            //Careega as categorias favoritas do utilizador
            foreach (var cat in _context.CategoriasFavoritas)
            {
                if(_context.CategoriasFavoritas.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id) != null)
                    Perfil.IdUtilizadorNavigation.CategoriasFavorita.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id).IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
            }

            //Carrega os ultimos 5 bilhetes do utilizador
            if(_context.Bilhetes.FirstOrDefault(b=> b.IdUtilizador == Perfil.IdUtilizador) != null)
            {
                List<Bilhete> list = new List<Bilhete>();

                foreach(Bilhete bilhete in _context.Bilhetes.Include(b => b.IdSessaoNavigation.IdFilmeNavigation).Include(b=>b.IdSessaoNavigation.IdSalaNavigation))
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

        //Gerador de codigos qr atravez de string
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

        //Get
        //Editar as categorias favoritas do utilizador
        public IActionResult EditarCategoriasFavoritas()
        {
            if (HttpContext.Session.GetInt32("IdUtilizador") == null)
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            int id = (int)HttpContext.Session.GetInt32("IdUtilizador");

            var Perfil = _context.Perfils.Include(p=> p.IdUtilizadorNavigation).FirstOrDefault(m => m.IdUtilizador == id);

            if (Perfil == null)
            {
                return NotFound();
            }

            //Carrega as categorias favoritas do utilizador da base de dados
            foreach (CategoriasFavorita cat in _context.CategoriasFavoritas)
            {
                if (_context.CategoriasFavoritas.FirstOrDefault(f => f.IdCategoria == cat.IdCategoria && f.IdUtilizador == id) != null)
                {
                    cat.IdCategoriaNavigation = _context.Categoria.FirstOrDefault(c => c.IdCategoria == cat.IdCategoria);
                    Perfil.IdUtilizadorNavigation.CategoriasFavorita.Add(cat);                        
                }
            }

            //Carrega todas as categorias da nase de dados
            ViewBag.Categorias = _context.Categoria;

            return View(Perfil);
        }

        //Post
        //Editar as categorias favoritas do utilizador
        [HttpPost]
        public async Task<IActionResult> EditarCategoriasFavoritas(int[] IdCategorias)
        {
            int id = (int)HttpContext.Session.GetInt32("IdUtilizador");

            //Remove todas as categorias favoritas do utilizador
            foreach (CategoriasFavorita cat in _context.CategoriasFavoritas)
            {
                if (cat.IdUtilizador == id)
                    _context.Remove(cat);
            }

            //Adiciona as novas categorias favoritas
            foreach (int IdCat in IdCategorias)
            {               

                CategoriasFavorita aux = new CategoriasFavorita();
                aux.IdCategoria = IdCat;
                aux.IdUtilizador = id;

                Categorium cat = _context.Categoria.Find(IdCat);
                aux.IdCategoriaNavigation = cat;

                Utilizador ut = _context.Utilizadors.Find(id);
                aux.IdUtilizadorNavigation = ut;
             
                _context.Add(aux);              
            }

            await _context.SaveChangesAsync();          
            return Redirect("Perfil/"+id.ToString());
        }

        //Get
        //Efetua o Logout
        public IActionResult Logout()
        {
            //Apaga todas as cookies de sessão
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Index", "Home");
        }

        //Get
        //Erro na ativação de um utilizador
        public IActionResult ActivationError()
        {
            return View();
        }

        //Get
        //Aviso para a ativação de um utilizador
        public IActionResult ActivationWarning()
        {
            return View();
        }

        //Get
        //Ativação de um utilizador
        [Route("Utilizadors/Activate/{*code}")]
        public async Task<IActionResult> Activate(string code)
        {
            //encontra o utilizador atravez do seu codigo unico
            Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.ActivationCode == code);

            if (user != null)
            {
                //Ativa o utilizador
                user.ActivationCode = "Activated";

                _context.Update(user);

                await _context.SaveChangesAsync();
                               
                return View();
            }

            return RedirectToAction("ActivationError", "Utilizadors");

        }

        // POST: Utilizadors/Register
        //Registo do utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,UserPassWord,IdUtilizador,IdGrupo")] Utilizador utilizador, [Bind("Nome,Email,DataNascimento,Telemovel")] Perfil perfil)
        {
            //Verifica se o username ja existe
            if (_context.Utilizadors.Where(u => u.UserName == utilizador.UserName).Count() != 0)
                ModelState.AddModelError("UserName", "Nome de utilizador ja existente");

            //Verifica se o email ja existe
            if (_context.Perfils.Where(u => u.Email == perfil.Email).Count() != 0)
                ModelState.AddModelError("Perfil.Email", "email ja registrado");

            if (ModelState.IsValid)
            {
                //Passa a password do utilizador a hash
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;
                utilizador.IdGrupo = 1;

                //Gera o codigo unico para ativação
                utilizador.ActivationCode = GenerateNewCode(25);

                _context.Add(utilizador);

                await _context.SaveChangesAsync();
                perfil.IdUtilizador = utilizador.IdUtilizador;
                perfil.IdUtilizadorNavigation = utilizador;

                _context.Add(perfil);
                await _context.SaveChangesAsync();

                //Envia o email para a ativação do utilizador
                EnviaEmail(perfil.Email,utilizador.ActivationCode);
                return RedirectToAction(nameof(ActivationWarning));
            }
            ViewData["IdGrupo"] = new SelectList(_context.GrupoPermissoes, "IdGrupo", "NomeGrupo", utilizador.IdGrupo);
            return View(utilizador);
        }

        //String para hash
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

        //Post
        //Login do utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("UserName,UserPassWord")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;

                //Carrega o utilizador com o username e password inseridos
                Utilizador user = _context.Utilizadors.FirstOrDefault(u => u.UserName == utilizador.UserName && u.UserPassWord == utilizador.UserPassWord);

                //Verifica se o utilizsdor existe
                if (user == null){
                    ModelState.AddModelError("UserName", "Utilizador ou password erradas!");
                    return View(utilizador);
                }

                //verifica se o utilizadoe esta ativado
                if(user.ActivationCode != "Activated")
                {
                    ModelState.AddModelError("UserName", "Utilizador não confirmado");
                    return View(utilizador);
                }

                //cria as cookies de sessão
                HttpContext.Session.SetInt32("IdUtilizador", user.IdUtilizador);
                HttpContext.Session.SetString("NomeUtilizador", user.UserName);

                string permissoes = null;
                
                //carrega as permissões para uma string
                foreach (var lista in _context.ListaPermissoes)
                {
                    if (lista.IdGrupo == user.IdGrupo)
                    {
                        permissoes += lista.IdPermissao.ToString();
                    }
                }

                //cria uma cookie com as permissões do utilizador
                if(permissoes != null)
                HttpContext.Session.SetString("Permissoes", permissoes);


                return RedirectToAction("index","Home");
            }           
            return View(utilizador);
        }

        //Get
        //Editar perfil do utilizador
        [HttpGet]
        public IActionResult EditarPerfil(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            if (id != HttpContext.Session.GetInt32("IdUtilizador"))
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            Perfil perfil = _context.Perfils.Include(p=>p.IdUtilizadorNavigation).FirstOrDefault(p => p.IdUtilizador == id);
            return View(perfil);
        }

        //Post
        //Editar perfil do utilizador
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(int id, Perfil perfil)
        {
            if (id != perfil.IdUtilizador)
            {
                return NotFound();
            }

            if (id != HttpContext.Session.GetInt32("IdUtilizador"))
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            Utilizador utilizador = _context.Utilizadors.Find(id);

            //Verifica se o nome de utilizador não esta em utilização
            if (perfil.IdUtilizadorNavigation.UserName != utilizador.UserName && _context.Utilizadors.FirstOrDefault(u=> u.UserName == perfil.IdUtilizadorNavigation.UserName) != null)
            {
                ModelState.AddModelError("IdUtilizadorNavigation.UserName", "Já existe um utilizador com esse UserName");
            }

            ModelState.Remove("IdUtilizadorNavigation.UserPassWord");

            if (ModelState.IsValid)
            {              
                utilizador.UserName = perfil.IdUtilizadorNavigation.UserName;
                perfil.IdUtilizadorNavigation = utilizador;

                try
                {
                    _context.Update(utilizador);
                    _context.Update(perfil);                 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfilExists(perfil.IdUtilizador))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //Atualiza a cookie com o nome de utilizador
                HttpContext.Session.SetString("NomeUtilizador", utilizador.UserName);

                return Redirect("/utilizadors/perfil/" + id.ToString());            
            }
            return View(perfil);
        }


        //Get
        //Repor password esquecida
        public IActionResult ForgotPassword()
        {
            return View();
        }


        //Post
        //Repor password esquecida
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            Perfil perfil = _context.Perfils.FirstOrDefault(p => p.Email == email);

            if(perfil != null)
            {                        
                Utilizador utilizador = await _context.Utilizadors.FindAsync(perfil.IdUtilizador);

                //Gera codigo para mudar a password
                utilizador.ActivationCode = GenerateNewCode(25);

                _context.Update(utilizador);
                await _context.SaveChangesAsync();

                //Envia email com o codigo
                EnviaEmailPassword(perfil.Email, utilizador.ActivationCode);              
            }

            //notificação
            _notyf.Success("email enviado!");

            return RedirectToAction("Index", "Home");
        }

        //Get
        //Mudar a password
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            if (id != HttpContext.Session.GetInt32("IdUtilizador"))
            {
                return RedirectToAction("PermissionDenied", "BackOffice");
            }

            Utilizador utilizador = await _context.Utilizadors.FindAsync(id);

            if(utilizador == null)
            {
                return NotFound();
            }

            //Gera codigo para mudar a password
            utilizador.ActivationCode = GenerateNewCode(25);

            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            //Envia email com o codigo
            EnviaEmailPassword(_context.Perfils.Find(id).Email, utilizador.ActivationCode);

            //Termina a sessão
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            //notificação
            _notyf.Success("email enviado!");

            return RedirectToAction("Index", "Home");
        }

        private bool PerfilExists(int id)
        {
            return _context.Perfils.Any(e => e.IdUtilizador == id);
        }


        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadors.Any(e => e.IdUtilizador == id);
        }

        //Envia email
        public void EnviaEmailPassword(string email, string code)
        {
          
            string Destino, Assunto, Mensagem;

            string Url = "https://localhost:44341/Utilizadors/ConfirmChangePassword/" + code;

            Destino = email;
            Assunto = "CinemaX - Alterar password";
            Mensagem = "<h1>Pedido de alteração de password</h1> <br/>Foi feito um pedido de alteração de password, altere a password para continuar a usufruir do website" +
            "<br/><a href=\"" + Url + "\">Clique aqui para alterar a password</a>";
            TesteEnvioEmail(Destino, Assunto, Mensagem).GetAwaiter();
        }

        //Envia email
        public void EnviaEmail(string email, string code)
        {            

            string Destino, Assunto, Mensagem;

            string Url = "https://localhost:44341/Utilizadors/Activate/"+code;

            Destino = email;
            Assunto = "Confirme o registo no website CinemaX";
            Mensagem = "<h1>Bem vindo ao CinemaX</h1> <br/>A sua conta foi criada com sucesso, confirme a sua conta para poder começar a usufruir do nosso website" +
            "<br/><a href=\""+Url+"\">Clique aqui para confirmar a sua conta</a>";
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

        //Gerador de codigos unicos
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
