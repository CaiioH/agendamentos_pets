using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetApi.Data;
using PetApi.Infrastructure;
using PetApi.Repository;
using PetApi.Services;
using PetApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configurar banco de dados
//Evita erros de concorrência
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 29))));

// Configura HttpClient para API externa The Dog API
builder.Services.AddHttpClient("DogAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TheDogApi:BaseUrl"]); // URL base correta
    client.DefaultRequestHeaders.Add("Accept", "application/json"); // Define o tipo de resposta esperada
})
.ConfigureHttpClient(client =>
{
    var apiKey = builder.Configuration["TheDogApi:ApiKey"];
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new Exception("API Key não configurada no appsettings.json!");
    }

    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
});

// Configura HttpClient para API externa The Cat API
builder.Services.AddHttpClient("CatAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TheCatApi:BaseUrl"]); // URL base correta
    client.DefaultRequestHeaders.Add("Accept", "application/json"); // Define o tipo de resposta esperada
})
.ConfigureHttpClient(client =>
{
    var apiKey = builder.Configuration["TheCatApi:ApiKey"];
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new Exception("API Key não configurada no appsettings.json!");
    }

    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<RacaService>();
builder.Services.AddSingleton<RabbitMqConnection>(); 
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.EnableAnnotations();
});

var app = builder.Build();

// **Roda as migrações automaticamente**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // Aplica as migrações automaticamente
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

var consumerService = app.Services.GetRequiredService<IRabbitMqService>();
consumerService.ConsumeMessages();

app.Run();

