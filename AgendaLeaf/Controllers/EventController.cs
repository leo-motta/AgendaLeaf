using AgendaLeaf.Data;
using AgendaLeaf.Models;
using AgendaLeaf.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace AgendaLeaf.Controllers
{
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly AgendaLeafContext _context;

        public EventController(ILogger<EventController> logger, AgendaLeafContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            if (idClaim != null)
            {
                ViewBag.OwnerId = idClaim.Value.ToString().ToUpper();
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

        public async Task<IActionResult> Edit(Guid id)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            if (idClaim != null)
            {
                ViewBag.OwnerId = idClaim.Value.ToString().ToUpper();
                ViewBag.Users = new MultiSelectList(_context.Users, "Id", "Name");
            }

            System.Diagnostics.Debug.WriteLine($"EventId:{id}\n\n");
            var EventObj = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (EventObj != null)
            {
                List<Guid> userIds = await _context.Users.Select(u => u.Id).ToListAsync();
                System.Diagnostics.Debug.WriteLine($"Event:{EventObj.Name}");
                System.Diagnostics.Debug.WriteLine($"\nCount: {userIds.Count}\n");
                foreach (var user in userIds)
                {
                    System.Diagnostics.Debug.WriteLine($"{user}");
                }
                EventViewModel viewModel = new EventViewModel { Event = EventObj, UsersId = userIds };
                return View(viewModel);
            }
            ViewData["ErrorMessage"] = "Erro";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Event,UsersId")] EventViewModel eventViewModel)
        {
            try
            {
                var newEvent = new Event
                {
                    Id = eventViewModel.Event.Id,
                    Name = eventViewModel.Event.Name,
                    Description = eventViewModel.Event.Description,
                    Date = eventViewModel.Event.Date,
                    Type = eventViewModel.Event.Type,
                    OwnerId = eventViewModel.Event.OwnerId
                };

                bool hasAny = await _context.Events.AnyAsync(x => x.Id == newEvent.Id);
                if(hasAny)
                {
                    _context.Events.Update(newEvent);
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

        public async Task<IActionResult> Delete(Guid id)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            if (idClaim != null)
            {
                ViewBag.OwnerId = idClaim.Value.ToString().ToUpper();
                ViewBag.Users = new MultiSelectList(_context.Users, "Id", "Name");
            }

            //System.Diagnostics.Debug.WriteLine($"EventId:{id}\n\n");
            var EventObj = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (EventObj != null)
            {
                List<Guid> userIds = await _context.Users.Select(u => u.Id).ToListAsync();
                EventViewModel viewModel = new EventViewModel { Event = EventObj, UsersId = userIds };
                return View(viewModel);
            }
            ViewData["ErrorMessage"] = "Erro";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid? Id)
        {
            System.Diagnostics.Debug.WriteLine($"Delete: {Id}");
            try
            {
                var EventObj = await _context.Events.FirstOrDefaultAsync(e => e.Id == Id);
                if (EventObj != null)
                {
                    _context.Events.Remove(EventObj);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                ViewData["ErrorMessage"] = "Erro ao deletar";
            }
            return View();
        }
    }
}