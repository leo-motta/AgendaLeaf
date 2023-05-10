using AgendaLeaf.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgendaLeaf.Data;

namespace AgendaLeaf.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AgendaLeafContext _context;

        public HomeController(ILogger<HomeController> logger, AgendaLeafContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (emailClaim != null)
            {
                ViewBag.email = emailClaim.Value;
            }

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            if(idClaim != null)
            {
                var UserId = new Guid(idClaim.Value.ToString().ToUpper());
                var Events = await _context.Events.Where(e => e.OwnerId == UserId).ToListAsync();
                /*
                System.Diagnostics.Debug.WriteLine($"\nEvents.Count: {Events.Count}\n");
                foreach (var e in Events)
                {
                    //System.Diagnostics.Debug.WriteLine($"{e.Name}");
                }
                */
                return View(Events);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Access");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}