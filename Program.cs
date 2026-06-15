 using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CampusHub.Data;
using CampusHub.Models;
using CampusHub.Web.Services;
using System.Net;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration (IMPORTANT: ApplicationUser)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<CampusDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});


builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<EventApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/"); // Votre port API
});
builder.Services.AddHttpClient<ClubApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/");
})
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new CookieContainer()
    });
builder.Services.AddHttpClient<AnnouncementApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/");
});
builder.Services.AddHttpClient<RoomReservationApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/");
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusDbContext>();

    
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();
// Important order
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();