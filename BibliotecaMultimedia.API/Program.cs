using BibliotecaMultimedia.API.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Agregar middleware
builder.Services.AddGlobalException();

// Agregar Configuracion de Identity
builder.Services.AddIdentityServices(builder.Configuration);

// Agregar Swagger
builder.Services.AddSwaggerService();

// Agregar Verionamiento
builder.Services.AddApiVersioningConfig();

// Agregar Postgres e Interceptors
builder.Services.AddInterceptors();
builder.Services.AddDbPostgres(builder.Configuration);

// Agregar Validators
builder.Services.AddValidations();

// Agregar Repositories
builder.Services.AddRepositories();

// Agregar Services
builder.Services.AddServices();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Genera el archivo json
    app.MapScalarApiReference(); // Levanta la nueva UI en /scalar/v1
}

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();