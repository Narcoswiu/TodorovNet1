using Microsoft.EntityFrameworkCore;
using TodorovNet.Models;

namespace TodorovNet.Data
{
    public class AppDbContext : DbContext
    {
        // Конструктор – тук подаваме настройките от Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблици в базата
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<Penalty> Penalties => Set<Penalty>();

    }
}
