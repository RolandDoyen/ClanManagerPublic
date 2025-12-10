using ClanManager.BLL.BLL;
using ClanManager.BLL.Interfaces;
using ClanManager.BLL.Seed;
using ClanManager.BLL.Services;
using ClanManager.DAL;
using ClanManager.WEB.Services;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Fatal)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Fatal)
        .Enrich.FromLogContext()
        .WriteTo.File("Logs/log-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();

    // DB Context
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ));

    // Dependency injections
    builder.Services.AddTransient<IClanBLL, ClanBLL>();
    builder.Services.AddTransient<IUserBLL, UserBLL>();

    // Services
    builder.Services.AddHttpContextAccessor();

    // Session Service
    builder.Services.AddScoped<ISessionService, SessionService>();

    // Admin Services
    builder.Services.AddScoped<IDatabaseService, DatabaseService>();

    // Mapper
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Logger
    builder.Host.UseSerilog();


    var app = builder.Build();

    // Seed initial data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        await DataSeeder.SeedAsync(context);
    }

    // Resource
    var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("fr") };

    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("en"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error500");
        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}"
    );

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error during application startup: {ex.Message}");
}