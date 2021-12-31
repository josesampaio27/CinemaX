﻿using System;
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

        // GET: Utilizadors
        public async Task<IActionResult> Index()
        {
            var cinemaXContext = _context.Perfils.Include(u => u.IdUtilizadorNavigation.IdGrupoNavigation);
            return View(await cinemaXContext.ToListAsync());
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

            if (Perfil == null)
            {
                return NotFound();
            }

            return View(Perfil);
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
        public async Task<IActionResult> Login([Bind("UserName,UserPassWord")] Utilizador utilizador)
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


                return RedirectToAction(nameof(Index));
            }           
            return View(utilizador);
        }

        public IActionResult EditarPerfil(int? id)
        {
            Perfil perfil = _context.Perfils.FirstOrDefault(p => p.IdUtilizador == id);

            return View(perfil);
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
            catch (Exception ex)
            {
                throw ex;
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
