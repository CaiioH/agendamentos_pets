using AgendaApi.Data;
using Microsoft.Extensions.DependencyInjection;
using AgendaApi.Infrastructure;
using AgendaApi.Repository;
using AgendaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 29))));

// Registra serviços corretamente no container principal
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAgendaService, AgendaService>();
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

// Configure o pipeline apenas após construir o app
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var consumerService = app.Services.GetRequiredService<IRabbitMqService>();
consumerService.ConsumeMessages();


app.Run();