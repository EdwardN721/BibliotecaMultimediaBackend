using BibliotecaMultimedia.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Agregar Swagger
builder.Services.AddSwaggerService();

// Agregar Postgres
builder.Services.AddInterceptors();
builder.Services.AddDbPostgres(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
