using AgendaLeaf.Data;
using AgendaLeaf.Models;
using AgendaLeaf.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
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
                //System.Diagnostics.Debug.WriteLine($"USERID: {userid}");
                ViewBag.Users = new MultiSelectList(_context.Users, "Id", "Name");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind("Event,UsersId")] EventViewModel eventViewModel)
        {
            try
            {
                var newEvent = new Event
                {
                    Id = Guid.NewGuid(),
                    Name = eventViewModel.Event.Name,
                    Description = eventViewModel.Event.Description,
                    Date = eventViewModel.Event.Date,
                    Type = eventViewModel.Event.Type,
                    OwnerId = eventViewModel.Event.OwnerId
                };
                _context.Events.Add(newEvent);

                foreach (var userId in eventViewModel.UsersId)
                {
                    var Upper = userId.ToString().ToUpper();

                    var newEventParticipant = new EventParticipant
                    {
                        Id = Guid.NewGuid(),
                        EventId = newEvent.Id,
                        UserId = userId
                    };

                    System.Diagnostics.Debug.WriteLine($"{newEventParticipant.Id} + {newEventParticipant.EventId} + {newEventParticipant.UserId}");
                    _context.EventParticipants.Add(newEventParticipant);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Erro ao salvar";
                System.Diagnostics.Debug.WriteLine($"exception: {ex}");
            }
                
            ViewData["ErrorMessage"] = "Erro";
            return View();
        }
    }
}
