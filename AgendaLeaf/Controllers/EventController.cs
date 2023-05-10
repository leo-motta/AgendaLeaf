using AgendaLeaf.Data;
using AgendaLeaf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AgendaLeaf.Controllers
{
    public class EventController : Controller
    {
        private readonly AgendaLeafContext _context;

        public EventController(AgendaLeafContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(emailClaim != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == emailClaim.Value);
                var userid = user?.Id.ToString().ToUpper();
                ViewBag.OwnerId = userid;
                System.Diagnostics.Debug.WriteLine($"USERID: {userid}");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Event ev)
        {
            if (ev != null)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"EVENT USERID: {ev.OwnerId} \n\n\n");
                    System.Diagnostics.Debug.WriteLine($"EVENT NAME: {ev.Name} \n\n\n");
                    _context.Events.Add(ev);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = "Erro ao salvar";
                    System.Diagnostics.Debug.WriteLine($"exception: {ex}");
                }
            }
            return View();
        }
    }
}
