using InmobiliariaAPI.Interfaces;
using InmobiliariaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InmobiliariaAPI.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configurar URLs
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5043", "http://*:5000", "https://*:5043");

// Registrar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexión a base de datos MySQL
builder.Services.AddDbContext<Context>(options =>
    options.UseMySql(
        builder.Configuration["ConnectionString"],
        new MariaDbServerVersion(new Version(10, 4, 32))
    )
);

// Inyección del servicio de Propietario
builder.Services.AddScoped<IPropietarioService, PropietarioService>();

// Configuración de JWT usando la misma clave que el login
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenAuthentication:SecretKey"]))
    };
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();