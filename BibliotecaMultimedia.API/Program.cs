using System.Text.Json.Serialization;
using BibliotecaMultimedia.API.Extensions;
using BibliotecaMultimedia.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
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

// Agregar Service Bus y Blobstorage
builder.Services.AddExternalServices(builder.Configuration);

// Agregar Cors
builder.Services.AddCorsConfiguration(builder.Configuration);

var app = builder.Build();

try
{
    await DatabaseSeeder.SeedCatalogoAsync(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "No se pudo ejecutar el sembrado del catálogo en el arranque.");
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Genera el archivo json
    app.MapScalarApiReference(); // Levanta la nueva UI en /scalar/v1
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();