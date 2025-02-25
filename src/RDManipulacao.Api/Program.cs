using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RDManipulacao.Infrastructure.Data;
using RDManipulacao.Infrastructure.Interfaces;
using RDManipulacao.Infrastructure.Repositories;
using RDManipulacao.Application.Interfaces;
using RDManipulacao.Application.Services;
using RDManipulacao.Domain.Configurations; // Para YouTubeApiSettings
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Configura��o de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configura��o do DbContext com SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar as op��es do YouTubeApiSettings a partir do appsettings.json
builder.Services.Configure<YouTubeApiSettings>(builder.Configuration.GetSection("YouTubeApiSettings"));

// Registrar o cliente Refit para a API do YouTube, utilizando a URL base definida nas configura��es
builder.Services.AddRefitClient<IYouTubeApi>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.Configuration["YouTubeApiSettings:BaseUrl"] ?? string.Empty));

// Registro dos demais servi�os e reposit�rios
builder.Services.AddScoped<IYouTubeService, YouTubeService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

// Configura��o do Swagger e endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RDManipulacao.API", Version = "v1" });
});

// Adiciona os controllers
builder.Services.AddControllers();

var app = builder.Build();

// Realiza a migra��o do banco de dados no in�cio da aplica��o
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configura��o do Swagger para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RDManipulacao.API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
