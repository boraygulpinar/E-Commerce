using E_Commerce.Server.Context;
using E_Commerce.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS Yap�land�rmas�n� ekliyoruz
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // T�m kaynaklardan eri�ime izin verir
              .AllowAnyMethod()   // T�m HTTP metodlar�na izin verir
              .AllowAnyHeader();  // T�m header'lara izin verir
    });
});

// Database Connection
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servisler
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI yap�land�rmas�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication yap�land�rmas�
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

Console.WriteLine($"Secret Key: {builder.Configuration["JwtSettings:Secret"]}");
Console.WriteLine($"Issuer: {builder.Configuration["JwtSettings:Issuer"]}");
Console.WriteLine($"Audience: {builder.Configuration["JwtSettings:Audience"]}");



var app = builder.Build();

// Static files ve CORS middleware'i s�ras�n� do�ru yap�yoruz
app.UseCors("AllowAll");  // CORS middleware'i ekliyoruz

app.UseDefaultFiles();
app.UseStaticFiles();

// HTTP request pipeline'�n� yap�land�r�yoruz
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
