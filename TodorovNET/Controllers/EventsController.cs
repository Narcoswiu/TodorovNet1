using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodorovNet.Data;
using TodorovNet.Models;

namespace TodorovNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Event model)
        {
            _db.Events.Add(model);
            await _db.SaveChangesAsync();
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events.ToListAsync();
            return Ok(events);
        }
    }
}
