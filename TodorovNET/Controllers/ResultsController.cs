using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TodorovNet.Data;
using TodorovNet.Hubs;
using TodorovNet.Models;

namespace TodorovNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<TimingHub> _hub;

        public ResultsController(AppDbContext db, IHubContext<TimingHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> AddResult([FromBody] Result model)
        {
            _db.Results.Add(model);
            await _db.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("NewResult", new
            {
                model.Id,
                model.ParticipantId,
                model.EventId,
                model.LapTime,
                model.IsFinal
            });

            return Ok(model);
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetResults(int eventId)
        {
            var results = await _db.Results
                .Include(r => r.Participant)
                .Where(r => r.EventId == eventId)
                .ToListAsync();

            // Сортиране в паметта, след като данните са изтеглени
            results = results
                .OrderBy(r => r.LapTime)
                .ToList();

            return Ok(results);
        }


        [HttpGet("standings/{eventId}")]
        public async Task<IActionResult> GetStandings(int eventId)
        {
            var penalties = await _db.Penalties
                .Where(p => p.EventId == eventId)
                .GroupBy(p => p.ParticipantId)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Seconds));

            var raw = await _db.Results
                .Include(r => r.Participant)
                .Where(r => r.EventId == eventId && r.IsFinal)
                .ToListAsync();

            // (по избор) ако има няколко финални за участник – вземи най-бързия
            raw = raw.GroupBy(r => r.ParticipantId)
                     .Select(g => g.OrderBy(x => x.LapTime).First())
                     .ToList();

            var rows = raw
                .Select(r =>
                {
                    var penSec = penalties.TryGetValue(r.ParticipantId, out var s) ? s : 0;
                    var baseSpan = r.LapTime;
                    var adjustedSpan = baseSpan + TimeSpan.FromSeconds(penSec);

                    return new
                    {
                        r.ParticipantId,
                        Number = r.Participant?.Number ?? 0,
                        Name = r.Participant?.Name ?? "",
                        Team = r.Participant?.Team ?? "",
                        BaseSpan = baseSpan,
                        PenaltySeconds = penSec,
                        AdjustedSpan = adjustedSpan,
                        r.IsFinal
                    };
                })
                .OrderBy(x => x.AdjustedSpan) // ✅ истински TimeSpan
                .Select(x => new TodorovNet.Models.Dtos.StandingRow
                {
                    ParticipantId = x.ParticipantId,
                    Number = x.Number,
                    Name = x.Name,
                    Team = x.Team,
                    LapTime = x.BaseSpan.ToString(@"hh\:mm\:ss"),
                    PenaltySeconds = x.PenaltySeconds,
                    AdjustedLapTime = x.AdjustedSpan.ToString(@"hh\:mm\:ss"),
                    IsFinal = x.IsFinal
                })
                .ToList();

            return Ok(rows);
        }



    }
}
