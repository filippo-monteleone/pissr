using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using RestAPI_Pissr;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(options => options.UseLazyLoadingProxies().UseInMemoryDatabase("Test"));

builder.Services.AddSingleton<BackgroundMqttClient>();
builder.Services.AddSingleton<IHostedService, BackgroundMqttClient>(ServiceProvider =>
    ServiceProvider.GetService<BackgroundMqttClient>());


builder.Services.AddSingleton<IHostedService, BackgroundAcquaRimanente>();


builder.Services.AddTransient<IDataService<AziendaAgricolaDTO>, AziendaAgricolaService>();
builder.Services.AddTransient<IDataService<AziendaIdricaDTO>, AziendaIdricaService>();
builder.Services.AddTransient<IDataService<CampoDTO>, CampoService>();
builder.Services.AddTransient<IDataService<DispositivoDTO>, DispositivoService>();


builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IGenericRepository<AziendaAgricola>, GenericRepository<AziendaAgricola>>();
builder.Services.AddTransient<IGenericRepository<AziendaIdrica>, GenericRepository<AziendaIdrica>>();
builder.Services.AddTransient<IGenericRepository<Campo>, GenericRepository<Campo>>();
builder.Services.AddTransient<IGenericRepository<Dispositivo>, GenericRepository<Dispositivo>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
