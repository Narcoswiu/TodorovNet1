using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TodorovNet.Data;
using TodorovNet.Models;

namespace TodorovNet.Controllers
{
    public class PenaltiesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<TodorovNet.Hubs.TimingHub> _hub;

        public PenaltiesController(AppDbContext db, IHubContext<TodorovNet.Hubs.TimingHub> hub)
        {
            _db = db; _hub = hub;
        }

        // /Penalties?eventId=1  (списък наказания за събитие)
        public async Task<IActionResult> Index(int eventId)
        {
            ViewBag.EventId = eventId;
            var items = await _db.Penalties
                .Include(p => p.Participant)
                .Where(p => p.EventId == eventId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(items);
        }

        // Форма за въвеждане: /Penalties/Create?eventId=1
        public async Task<IActionResult> Create(int eventId)
        {
            ViewBag.EventId = eventId;
            ViewBag.Participants = await _db.Participants
                .OrderBy(x => x.Number)
                .ToListAsync();
            return View(new Penalty { EventId = eventId, Type = PenaltyType.Gps });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Penalty model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EventId = model.EventId;
                ViewBag.Participants = await _db.Participants.OrderBy(x => x.Number).ToListAsync();
                return View(model);
            }

            _db.Penalties.Add(model);
            await _db.SaveChangesAsync();

            // Лайв сигнал за обновяване на класиране
            await _hub.Clients.Group($"event_{model.EventId}")
                .SendAsync("PenaltyUpdated", new { model.EventId, model.ParticipantId });

            return RedirectToAction(nameof(Index), new { eventId = model.EventId });
        }

        // по желание: Edit/Delete (класически CRUD)
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Penalties.Include(x => x.Participant).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();
            return View(p);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _db.Penalties.FindAsync(id);
            if (p == null) return NotFound();
            var eventId = p.EventId;
            _db.Penalties.Remove(p);
            await _db.SaveChangesAsync();
            await _hub.Clients.Group($"event_{eventId}").SendAsync("PenaltyUpdated", new { EventId = eventId });
            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
