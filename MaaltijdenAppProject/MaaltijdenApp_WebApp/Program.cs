using MaaltijdenApp_Core.services;
using MaaltijdenApp_EFSQL_MaaltijdenDb;
using MaaltijdenApp_EFSQL_MaaltijdenDb.services;
using MaaltijdenApp_IFSQL_IdentityDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MaaltijdenAppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["Data:MaaltijdenDataStore:ConnectionString"]);
});

builder.Services.AddTransient<ICanteenRepository, EFCanteenRepository>();
builder.Services.AddTransient<IEmployeeRepository, EFEmployeeRepository>();
builder.Services.AddTransient<IMealPackageRepository, EFMealPackageRepository>();
builder.Services.AddTransient<IProductRepository, EFProductRepository>();
builder.Services.AddTransient<IStudentRepository, EFStudentRepository>();

// Connection to Identity DB
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["Data:IdentityDataStore:ConnectionString"]);
});

// Identity config for Identity DB
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 5;
}).AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
