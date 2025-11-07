using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using TodorovNet.Data;
using TodorovNet.Models;

namespace TodorovNet.Controllers
{
    [Authorize(Roles = "Organizer")]
    public class ImportController : Controller
    {
        private readonly AppDbContext _db;
        public ImportController(AppDbContext db) { _db = db; }

        // GET: /Import/Participants?eventId=1
        [HttpGet]
        public async Task<IActionResult> Participants(int eventId)
        {
            var ev = await _db.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == eventId);
            if (ev == null) return NotFound();
            ViewBag.Event = ev;
            return View();
        }

        // POST: /Import/Participants  (Excel .xlsx/.xls ИЛИ .csv)
        [HttpPost]
        public async Task<IActionResult> Participants(int eventId, IFormFile file)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (ev == null) return NotFound();

            if (file == null || file.Length == 0)
            {
                TempData["Err"] = "Моля, качи файл (CSV или Excel).";
                return RedirectToAction(nameof(Participants), new { eventId });
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            int imported = 0;

            try
            {
                if (ext == ".xlsx" || ext == ".xls")
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    using var stream = file.OpenReadStream();
                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    });

                    var table = ds.Tables[0];
                    foreach (DataRow row in table.Rows)
                    {
                        var nameObj = table.Columns.Contains("Name") ? row["Name"] : null;
                        var name = nameObj?.ToString()?.Trim();
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        int number = 0;
                        if (table.Columns.Contains("Number"))
                            int.TryParse(row["Number"]?.ToString(), out number);

                        var team = table.Columns.Contains("Team") ? (row["Team"]?.ToString()?.Trim() ?? "") : "";

                        _db.Participants.Add(new Participant
                        {
                            Name = name,
                            Number = number,
                            Team = team
                        });
                        imported++;
                    }

                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        TrimOptions = TrimOptions.Trim,
                        MissingFieldFound = null,
                        BadDataFound = null
                    };
                    using var stream = file.OpenReadStream();
                    using var sr = new StreamReader(stream);
                    using var csv = new CsvReader(sr, cfg);

                    var rows = csv.GetRecords<CsvRow>();
                    foreach (var r in rows)
                    {
                        if (string.IsNullOrWhiteSpace(r.Name)) continue;

                        _db.Participants.Add(new Participant
                        {
                            Name = r.Name.Trim(),
                            Number = r.Number,
                            Team = r.Team?.Trim() ?? ""
                        });
                        imported++;
                    }
                    await _db.SaveChangesAsync();
                }

                TempData["Msg"] = $"Импортирани участници: {imported}";
                return RedirectToAction(nameof(Participants), new { eventId });
            }
            catch (Exception ex)
            {
                TempData["Err"] = "Грешка при импорта: " + ex.Message;
                return RedirectToAction(nameof(Participants), new { eventId });
            }
        }

        public class CsvRow
        {
            public int Number { get; set; }
            public string Name { get; set; } = "";
            public string? Team { get; set; }
        }
    }
}
