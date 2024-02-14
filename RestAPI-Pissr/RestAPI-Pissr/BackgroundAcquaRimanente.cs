using System.Runtime.InteropServices.JavaScript;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using MQTTnet;
using MQTTnet.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

namespace RestAPI_Pissr
{
    public class BackgroundAcquaRimanente : BackgroundService
    {
        private readonly BackgroundMqttClient _BackgroundMqttClient;
        private DayOfWeek? _nextDay = null;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public BackgroundAcquaRimanente(BackgroundMqttClient backgroundMqttClient, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _BackgroundMqttClient = backgroundMqttClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                switch ((_nextDay, _BackgroundMqttClient.Date.DayOfWeek))
                {
                    case (null, >= 0):
                        _nextDay = _BackgroundMqttClient.Date.DayOfWeek;
                        Console.WriteLine("setting");
                        break;
                    case ( >= 0, >= 0) when _nextDay != _BackgroundMqttClient.Date.DayOfWeek:
                        // Entrando qua significa che e' appena passato un giorno
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var campoService = (CampoService)scope.ServiceProvider.GetRequiredService<IDataService<CampoDTO>>();

                            var campi = (await campoService.GetList()).ToList();

                            // Itero per ogni campo e mando al gestore corrispondente il messaggio di irrigazione
                            for (var i = 0; i < campi.Count(); i++)
                            {
                                var campo = campi[i];

                                var date = new DateTime(_BackgroundMqttClient.Date.Year,
                                    _BackgroundMqttClient.Date.Month, _BackgroundMqttClient.Date.Day, 23, 59, 59);

                                var soilMoisture = await campoService.UmiditaRimasta(campo.Id, date.AddDays(-1));

                                var irrTime = await campoService.GetIrriguoNetto(campo.Id, date, soilMoisture);

                                var text = JsonConvert.SerializeObject(new
                                {
                                    campo = campo.Id,
                                    umidita = soilMoisture,
                                    umidita_critica = campo.CriticalSoilMoisture,
                                    umidita_preferita = campo.Hum,
                                    tempo_irrigazione = irrTime
                                });

                                var applicationMessage = new MqttApplicationMessageBuilder()
                                    .WithTopic($"Azienda/{campo.AziendaAgricolaId}/Campo/{campo.Id}/Irrigazione")
                                    .WithPayload(text)
                                    .Build();
                                await _BackgroundMqttClient.mqttClient.PublishAsync(applicationMessage);
                            }
                        }

                        _nextDay = _BackgroundMqttClient.Date.DayOfWeek;
                        break;
                    default:
                        Console.Write("hoold");
                        break;
                }

                Console.WriteLine($"acqua {_BackgroundMqttClient.Date}");
                await Task.Delay(1000);
            }
        }
    }
}
