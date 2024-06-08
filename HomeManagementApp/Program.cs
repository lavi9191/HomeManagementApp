using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomeManagementApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Dodaj DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// U�ywaj AppUser
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Dodaj wsparcie dla r�l
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Dodaj logowanie
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Dodaj kod do utworzenia roli "Admin" podczas uruchamiania aplikacji
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Sprawd�, czy rola "Admin" istnieje, je�li nie - utw�rz j�
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("Rola 'Admin' zosta�a utworzona.");
        }
        else
        {
            logger.LogInformation("Rola 'Admin' ju� istnieje.");
        }

        // Dodaj pierwszego u�ytkownika jako administratora
        var adminEmail = "admin@o2.pl";
        var adminPassword = "zaq1@WSX";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("U�ytkownik admin@o2.pl zosta� utworzony i przypisano mu rol� 'Admin'.");
            }
            else
            {
                logger.LogError("Nie uda�o si� utworzy� u�ytkownika admin@o2.pl.");
                foreach (var error in createResult.Errors)
                {
                    logger.LogError($"B��d: {error.Description}");
                }
            }
        }
        else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("U�ytkownik admin@o2.pl ju� istnieje, przypisano mu rol� 'Admin'.");
        }
        else
        {
            logger.LogInformation("U�ytkownik admin@o2.pl ju� istnieje i ma przypisan� rol� 'Admin'.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"Wyst�pi� b��d podczas tworzenia u�ytkownika admina: {ex.Message}");
        throw;
    }
}


// Konfiguruj middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
