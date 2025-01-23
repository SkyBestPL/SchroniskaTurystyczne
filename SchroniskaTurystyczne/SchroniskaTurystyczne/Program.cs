using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchroniskaTurystyczne.Controllers;
using SchroniskaTurystyczne.Data;
using SchroniskaTurystyczne.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<MessageController>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireGuestRole", policy => policy.RequireRole("Guest"));
    options.AddPolicy("RequireExhibitorRole", policy => policy.RequireRole("Exhibitor"));
    options.AddPolicy("RequireAdminOrGuestRole", policy => policy.RequireRole("Admin", "Guest"));
    options.AddPolicy("RequireAdminOrExhibitorRole", policy => policy.RequireRole("Admin", "Exhibitor"));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.Run();
