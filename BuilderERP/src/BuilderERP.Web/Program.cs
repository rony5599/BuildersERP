using BuilderERP.Application;
using BuilderERP.Application.Common;
using BuilderERP.Domain.Entities;
using BuilderERP.Infrastructure;
using BuilderERP.Infrastructure.Identity;
using BuilderERP.Infrastructure.Persistence;
using BuilderERP.Shared.Middleware;
using BuilderERP.Web.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/builderERP-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.Configure<ProcurementSettings>(builder.Configuration.GetSection("ProcurementSettings"));
    builder.Services.AddSingleton<IUploadsPathProvider, UploadsPathProvider>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

    builder.Services.AddControllersWithViews();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "BuilderERP API",
            Version = "v1"
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "BuilderERP API v1"));
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var dbContext = provider.GetRequiredService<AppDbContext>();
        var roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedData.SeedAsync(dbContext, roleManager, userManager);
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    var uploadsPathProvider = app.Services.GetRequiredService<IUploadsPathProvider>();
    Directory.CreateDirectory(uploadsPathProvider.PhysicalRoot);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsPathProvider.PhysicalRoot),
        RequestPath = "/uploads"
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex) when (ex is not Microsoft.Extensions.Hosting.HostAbortedException)
{
    Log.Fatal(ex, "BuilderERP.Web terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
