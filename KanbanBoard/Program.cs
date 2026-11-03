using KanbanBoard.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registrerar MVC så att vi kan använda Controllers och Views.
builder.Services.AddControllersWithViews();

// Registrerar vår databaskontext i ASP.NET Cores
// dependency injection-system.
//
// UseSqlite anger att vi använder SQLite.
// Anslutningssträngen hämtas från appsettings.json.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// Hanterar fel på ett säkert sätt när applikationen
// inte körs i utvecklingsläge.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Aktiverar omdirigering till HTTPS om det är konfigurerat.
app.UseHttpsRedirection();

// Gör CSS, JavaScript och andra statiska filer tillgängliga.
app.MapStaticAssets();

// Aktiverar routing och auktorisering.
app.UseRouting();
app.UseAuthorization();

// Anger standardadressen för vår MVC-applikation.
// Startsidan öppnas via HomeController och Index.
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Boards}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();