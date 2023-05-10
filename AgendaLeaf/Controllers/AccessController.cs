using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AgendaLeaf.Models;
using AgendaLeaf.Data;
using Microsoft.EntityFrameworkCore;

namespace AgendaLeaf.Controllers
{
    public class AccessController : Controller
    {
        private readonly AgendaLeafContext _context;
        public AccessController(AgendaLeafContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if(claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AgendaLogin agendaLogin)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == agendaLogin.Email);

            if (currentUser != null && agendaLogin.Password == currentUser.Password)
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, agendaLogin.Email),
                    new Claim(ClaimTypes.UserData, currentUser.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme );

                AuthenticationProperties properties= new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = agendaLogin.KeepLoggedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Index","Home");
            }

            ViewData["ValidateMessage"] = "usuário não encontrado";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (user != null)
            {
                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "Access");
                } catch(Exception ex)
                {
                    ViewData["ErrorMessage"] = "Erro ao salvar";
                }
            }
            ViewData["ErrorMessage"] = "Erro de usuário inválido";
            return View();
        }
    }
}
