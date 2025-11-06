using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodorovNet.Data;

namespace TodorovNet.Controllers
{
    public class EventsUiController : Controller
    {
        private readonly AppDbContext _db;
        public EventsUiController(AppDbContext db) { _db = db; }

        // /EventsUi
        public async Task<IActionResult> Index()
            => View(await _db.Events.OrderByDescending(e => e.Date).ToListAsync());

        // /EventsUi/Participants/{id}
        public async Task<IActionResult> Participants(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();
            ViewBag.Event = ev;
            var list = await _db.Participants.OrderBy(p => p.Number).ToListAsync();
            return View(list);
        }
    }
    ///
}
