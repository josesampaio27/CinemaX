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

namespace CinemaX.Controllers
{
    public class UtilizadorsController : Controller
    {
        private readonly CinemaXContext _context;

        public UtilizadorsController(CinemaXContext context)
        {
            _context = context;
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
            return View();
        }

        public async Task<IActionResult> Perfil(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Perfil = await _context.Perfils
                .FirstOrDefaultAsync(m => m.IdUtilizador == id);
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

        // POST: Utilizadors/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,UserPassWord,IdUtilizador,IdGrupo")] Utilizador utilizador, [Bind("Nome,Email,DataNascimento,Telemovel")] Perfil perfil)
        {
            if (ModelState.IsValid)
            {
                string Hash = GetStringSha256Hash(utilizador.UserPassWord);
                utilizador.UserPassWord = Hash;
                utilizador.IdGrupo = 1;
                _context.Add(utilizador);
                await _context.SaveChangesAsync();
                perfil.IdUtilizador = utilizador.IdUtilizador;
                perfil.IdUtilizadorNavigation = utilizador;
                _context.Add(perfil);
                await _context.SaveChangesAsync();               
                return RedirectToAction(nameof(Index));
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

                HttpContext.Session.SetInt32("IdUtilizador", user.IdUtilizador);
                HttpContext.Session.SetString("NomeUtilizador", user.UserName);


                return RedirectToAction(nameof(Index));
            }           
            return View(utilizador);
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizadors.Any(e => e.IdUtilizador == id);
        }
    }
}
