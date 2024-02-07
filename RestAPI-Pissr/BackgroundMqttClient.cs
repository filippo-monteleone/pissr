using System.Runtime.InteropServices.JavaScript;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using RestAPI_Pissr.Models;
using RestSharp;

namespace RestAPI_Pissr;

public class BackgroundMqttClient : BackgroundService
{
    public DateTime Date { get; private set; } = DateTime.Now;
    public int Speed { get; set; } = 1;
    public IMqttClient mqttClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundMqttClient(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        mqttClient = new MqttFactory().CreateMqttClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            // Mando il quanta acqua rimane per quel giorno al gestore iot
            // Altrimenti inserisco nel db l'evento inviato dal gestore iot del dispositivo
            if (e.ApplicationMessage.Topic.Contains("Acqua"))
            {
                var aziendaId = int.Parse(System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment));
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var campoService = (CampoService)scope.ServiceProvider.GetRequiredService<IDataService<CampoDTO>>();
                    var aziendaAgService = (AziendaAgricolaService)scope.ServiceProvider.GetRequiredService<IDataService<AziendaAgricolaDTO>>();


                    var campi = (await campoService.GetList()).Where(x => x.AziendaAgricolaId == aziendaId).ToList();
                    var contratto = (await aziendaAgService.GetRichiesta(aziendaId, ContrattoStato.Accepted, Date));

                    var sum = 0d;

                    for (int i = 0; i < campi.Count; i++)
                    {
                        sum += await campoService.GetAcquaConsumata(campi[i].Id, Date);
                    }

                    var applicationMessage = new MqttApplicationMessageBuilder()
                        .WithTopic($"Azienda/{aziendaId}/Acqua")
                        .WithPayload($"acqua_rimanente: {contratto.Acqua - sum}")
                        .Build();

                    await mqttClient.PublishAsync(applicationMessage);
                }
            }
            else
            {
                #region Calcolo irrigazione
                var json = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);
                var evento = JsonConvert.DeserializeAnonymousType(json, new { Tipo = 0, Valore = "", DispositivoId = 0 });
                Console.WriteLine(evento);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dispositivoService = (DispositivoService)scope.ServiceProvider.GetRequiredService<IDataService<DispositivoDTO>>();

                    var dispositivo = await dispositivoService.GetById(evento.DispositivoId);

                    if (dispositivo is null)
                        return;

                    await dispositivoService.Insert(new EventoDto()
                    {
                        Tipo = (BusinessLogicLayer.Dtos.EventoTipo)evento.Tipo,
                        Tempo = Date,
                        Valore = evento.Valore,
                        DispositivoId = evento.DispositivoId,
                        CampoId = dispositivo.CampoId
                    });
                }
                #endregion
            }
        };

        await mqttClient.ConnectAsync(options);

        var mqttSubsOption = new MqttFactory().CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
            {
                f.WithTopic("Azienda/+/Acqua");
            })
            .WithTopicFilter(f =>
            {
                f.WithTopic("Azienda/+/Evento");
            })
            .Build();

        await mqttClient.SubscribeAsync(mqttSubsOption, CancellationToken.None);

        // loop infinito per simulare il passare del tempo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000 * Speed, stoppingToken);
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("Tempo")
                .WithPayload(Date.ToString())
                .Build();
            await mqttClient.PublishAsync(applicationMessage);
            //Date = Date.AddSeconds(1);
            Date = Date.AddHours(1);
        }
    }
}