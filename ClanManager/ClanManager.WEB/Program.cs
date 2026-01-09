using ClanManager.BLL.BLL;
using ClanManager.BLL.Interfaces;
using ClanManager.BLL.Seed;
using ClanManager.BLL.Services;
using ClanManager.DAL;
using ClanManager.DAL.Interfaces;
using ClanManager.DAL.Repositories;
using ClanManager.WEB.Middleware;
using ClanManager.WEB.Services;
using ClanManager.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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

    builder.Services.AddScoped<GlobalExceptionFilter>();

    builder.Services.Configure<MvcOptions>(options =>
    {
        options.Filters.AddService<GlobalExceptionFilter>();
    });

    // DB Context
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ));

    builder.Services.AddHttpContextAccessor();

    // Dependency injections
    builder.Services.AddTransient<IClanBLL, ClanBLL>();
    builder.Services.AddTransient<IUserBLL, UserBLL>();
    builder.Services.AddScoped<ISessionService, SessionService>();
    builder.Services.AddScoped<IDatabaseService, DatabaseService>();
    builder.Services.AddScoped<IValidationService, ValidationService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IClanRepository, ClanRepository>();

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

    // Application Insights
    builder.Services.AddApplicationInsightsTelemetry();


    var app = builder.Build();

    // Seed initial data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Database.Migrate();

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

        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

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