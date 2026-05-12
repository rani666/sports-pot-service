using Scalar.AspNetCore;
using sports_pot_service.Models;
using sports_pot_service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddHttpClient("events");
builder.Services.AddSingleton<EventStore>();
builder.Services.AddHostedService<EventSyncService>();
builder.Services.AddSingleton<SportsPotService>();
builder.Services.AddSingleton<JackpotConfigService>();
builder.Services.AddSingleton<JackpotPotService>();
builder.Services.AddSingleton<LiveJackpotService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();
