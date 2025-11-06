using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodorovNet.Data;
using TodorovNet.Models;

namespace TodorovNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ParticipantsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> AddParticipant([FromBody] Participant participant)
        {
            _db.Participants.Add(participant);
            await _db.SaveChangesAsync();
            return Ok(participant);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var participants = await _db.Participants.ToListAsync();
            return Ok(participants);
        }
    }
}
