using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TodorovNet.Data;
using TodorovNet.Hubs;
using TodorovNet.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=todorovnet.db"));

// Identity + UI
builder.Services
    .AddIdentity<AppUser, IdentityRole>(opt =>
    {
        opt.Password.RequiredLength = 6;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddRazorPages(); // за Identity UI
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(o => o.EnableForHttps = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseResponseCompression();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // /Identity/Account/...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<TimingHub>("/timingHub");

// Seed роли/потребител
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedAuth.EnsureSeedAsync(services);
}

app.Run();

public static class SeedAuth
{
    public static async Task EnsureSeedAsync(IServiceProvider sp)
    {
        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = sp.GetRequiredService<UserManager<AppUser>>();

        var organizerRole = "Organizer";
        if (!await roleMgr.RoleExistsAsync(organizerRole))
            await roleMgr.CreateAsync(new IdentityRole(organizerRole));

        var email = "organizer@todorov.net";
        var user = await userMgr.FindByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
            await userMgr.CreateAsync(user, "T0dorov!23"); // смени после
            await userMgr.AddToRoleAsync(user, organizerRole);
        }
    }
}
