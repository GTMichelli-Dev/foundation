using BasicWeigh.Web.Data;
using BasicWeigh.Web.Services;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
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

// DevExpress Reporting
builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator =>
{
    if (builder.Environment.IsDevelopment())
    {
        configurator.UseDevelopmentMode();
    }
    configurator.ConfigureReportDesigner(designerConfigurator => { });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
    {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

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

app.UseDevExpressControls();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
