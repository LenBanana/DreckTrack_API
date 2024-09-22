using System.Text;
using System.Text.Json;
using DreckTrack_API.AutoMapper.TypeConverter;
using DreckTrack_API.Controllers.AuthFilter;
using DreckTrack_API.Database;
using DreckTrack_API.Models;
using DreckTrack_API.Models.Entities;
using DreckTrack_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RawgSettings>(builder.Configuration.GetSection("Rawg"));
builder.Services.AddHttpClient("RawgClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Rawg:BaseUrl"] ?? throw new InvalidOperationException());
});
builder.Services.Configure<TestDiveSettings>(builder.Configuration.GetSection("TestDive"));
builder.Services.AddHttpClient("TestDiveClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TestDive:BaseUrl"] ?? throw new InvalidOperationException());
});


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Register the custom converter
        options.JsonSerializerOptions.Converters.Add(new CollectibleItemDtoConverter());
    });

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register Services
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<MailService>();

// Add Scopes
builder.Services.AddScoped<UserExistenceFilter>();

// Authentication with JWT
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new Exception("JWT Secret is missing");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configuration
var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Apply pending migrations
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log the error or handle it as needed
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Use CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();