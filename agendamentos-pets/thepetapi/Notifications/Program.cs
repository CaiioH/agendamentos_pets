using Notifications.Infrastructure;
using Notifications.Models;
using Notifications.Services;
using Notifications.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<RabbitMqConnection>(); 
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var consumerService = app.Services.GetRequiredService<IRabbitMqService>();
consumerService.ConsumeMessages();

app.Run();
