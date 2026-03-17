using BasicWeigh.Web.Data;
using BasicWeigh.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database provider switching
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "SQLite";
var connectionString = builder.Configuration.GetConnectionString(dbProvider)
    ?? "Data Source=BasicWeigh.db";

builder.Services.AddDbContext<ScaleDbContext>(options =>
{
    if (dbProvider == "MariaDB")
    {
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Scale service (simulated for now)
builder.Services.AddSingleton<SimulatedScaleService>();
builder.Services.AddSingleton<IScaleService>(sp => sp.GetRequiredService<SimulatedScaleService>());

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScaleDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Seed(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
