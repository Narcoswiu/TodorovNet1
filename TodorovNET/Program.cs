using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TodorovNet.Data;
using TodorovNet.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ✅ ВСИЧКИ услуги САМО преди Build()
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        // предотвратява циклични референции Participant <-> Results
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=todorovnet.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ⛔️ Нищо повече към builder.Services след този ред
var app = builder.Build();

// ✅ Middleware и маршрути след Build()
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapHub<TimingHub>("/timingHub");

app.Run();
