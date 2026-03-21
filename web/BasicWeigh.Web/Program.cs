using BasicWeigh.Web.Data;
using BasicWeigh.Web.Hubs;
using BasicWeigh.Web.Services;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using Microsoft.AspNetCore.Authentication.Cookies;
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

builder.Services.AddSignalR();
builder.Services.AddHostedService<ScaleBroadcastService>();

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

// DevExpress Reporting
builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator =>
{
    if (builder.Environment.IsDevelopment())
    {
        configurator.UseDevelopmentMode();
    }
    configurator.ConfigureReportDesigner(designerConfigurator =>
    {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
    });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
    {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageService());

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScaleDbContext>();
    context.Database.Migrate();
    DbInitializer.Seed(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseDevExpressControls();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

// Middleware: if UseLogin is off, skip auth. If on, enforce it.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Always allow: login page, static files, kiosk, scale API, SignalR
    if (path.StartsWith("/Account/") ||
        path.StartsWith("/css/") || path.StartsWith("/js/") || path.StartsWith("/images/") ||
        path.StartsWith("/_content/") || path.StartsWith("/favicon") ||
        path.StartsWith("/api/scale/") || path.StartsWith("/scaleHub") ||
        path.StartsWith("/api/setup/icon"))
    {
        await next();
        return;
    }

    // Kiosk access: check PIN if UseLogin is on
    if (path.StartsWith("/Kiosk") || path.StartsWith("/api/kiosk/"))
    {
        var db = context.RequestServices.GetRequiredService<ScaleDbContext>();
        var setup = db.AppSetup.First();
        if (setup.UseLogin)
        {
            var pin = context.Request.Query["pin"].FirstOrDefault()
                      ?? context.Request.Cookies["KioskPin"];
            if (pin != setup.KioskCode)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }
            // Set cookie so subsequent requests don't need ?pin=
            if (!context.Request.Cookies.ContainsKey("KioskPin"))
            {
                context.Response.Cookies.Append("KioskPin", pin, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });
            }
        }
        await next();
        return;
    }

    // Check if login is required
    var dbCheck = context.RequestServices.GetRequiredService<ScaleDbContext>();
    var appSetup = dbCheck.AppSetup.First();
    if (appSetup.UseLogin && !context.User.Identity!.IsAuthenticated)
    {
        context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(path)}");
        return;
    }

    // Role-based access
    if (appSetup.UseLogin && context.User.Identity!.IsAuthenticated)
    {
        var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";

        // Setup page: Admin only
        if (path.StartsWith("/Setup") && role != "Admin")
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access denied. Admin role required.");
            return;
        }

        // User management: Admin only
        if (path.StartsWith("/Account/Users") || path.StartsWith("/Account/CreateUser") ||
            path.StartsWith("/Account/EditUser") || path.StartsWith("/Account/ResetPassword") ||
            path.StartsWith("/Account/DeleteUser"))
        {
            if (role != "Admin")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied. Admin role required.");
                return;
            }
        }

        // Edit Tables: Manager or Admin only
        if (path.StartsWith("/MasterData") && role == "User")
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access denied. Manager or Admin role required.");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ScaleHub>("/scaleHub");

app.Run();
