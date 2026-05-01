using BibliotecaMultimedia.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Agregar middleware
builder.Services.AddGlobalException();

// Agregar Configuracion de Identity
builder.Services.AddIdentityServices();

// Agregar Swagger
builder.Services.AddSwaggerService();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();