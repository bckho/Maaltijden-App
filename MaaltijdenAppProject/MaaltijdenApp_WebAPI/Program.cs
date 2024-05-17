using MaaltijdenApp_Core.services;
using MaaltijdenApp_EFSQL_MaaltijdenDb;
using MaaltijdenApp_EFSQL_MaaltijdenDb.services;
using MaaltijdenApp_IFSQL_IdentityDb;
using MaaltijdenApp_WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("API v0.1", new OpenApiInfo { Title = "Maaltijden App API", Version = "v0.1" });
});

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<MealQuery>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Add Database connection with MaaltijdenAppDbContext
builder.Services.AddDbContext<MaaltijdenAppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["Data:MaaltijdenDataStore:ConnectionString"]);
});

// Add Identity Db connection
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["Data:IdentityDataStore:ConnectionString"]);
});

// Add database services
builder.Services.AddTransient<IStudentRepository, EFStudentRepository>();
builder.Services.AddTransient<IMealPackageRepository, EFMealPackageRepository>();
builder.Services.AddTransient<ICanteenRepository, EFCanteenRepository>();
builder.Services.AddTransient<IProductRepository, EFProductRepository>();
builder.Services.AddTransient<IStudentRepository, EFStudentRepository>();
builder.Services.AddTransient<IEmployeeRepository, EFEmployeeRepository>();

// Identity config for Identity DB
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.ValidateIssuer = false;
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["BearerTokens:Key"]));
});

// GraphQL configurations
builder.Services.AddGraphQLServer().AddQueryType<MealQuery>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(isOriginAllowed: _ => true)
        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
