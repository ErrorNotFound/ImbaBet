using Microsoft.EntityFrameworkCore;
using ImbaBetWeb.Data;
using Microsoft.AspNetCore.Identity;
using ImbaBetWeb.Models;
using ImbaBetWeb.Logic;
using System.Data;
using ImbaBetWeb.Services;
using System.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationContext>();

// forces session validation every minute
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
	options.ValidationInterval = TimeSpan.FromMinutes(1);
});

builder.Services.AddDbContext<ApplicationContext>(
    options =>
    {
        options.UseLazyLoadingProxies();
        options.UseSqlServer(connectionString);   
    });

builder.Services.AddScoped<BettingManager>();
builder.Services.AddScoped<GameManager>();
builder.Services.AddScoped<CommunityManager>();
builder.Services.AddScoped<DatabaseManager>();
builder.Services.AddScoped<SettingsManager>();
builder.Services.AddScoped<MatchPlanImportService>();

builder.Services.AddTransient<IEmailSender, EmailService>(i =>
                new EmailService(
                    builder.Configuration["EmailService:Host"]!,
                    builder.Configuration.GetValue<int>("EmailService:Port"),
                    builder.Configuration.GetValue<bool>("EmailService:EnableSSL"),
                    builder.Configuration["EmailService:UserName"]!,
                    builder.Configuration["EmailService:Password"]!
                )
            );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // make sure database is created and migrated
    var context = services.GetRequiredService<ApplicationContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    // make sure database is seeded with required data
    var databaseManager = services.GetRequiredService<DatabaseManager>();
    await databaseManager.InitialDatabaseSeedAsync();
}

app.Run();
