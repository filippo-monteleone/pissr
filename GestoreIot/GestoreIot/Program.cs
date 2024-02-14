// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

var data = new DateTime();
var campiCritici = new List<int>();

var mqttFactory = new MqttFactory();

IMqttClient client;

using (var mqttClient = mqttFactory.CreateMqttClient())
{

    var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

    await mqttClient.ConnectAsync(mqttClientOptions);

    client = mqttClient;

    #region Topic d'ascoltare
    var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        .WithTopicFilter(
            f =>
            {
                f.WithTopic($"Azienda/{args[0]}/Campo/#");
            }).WithTopicFilter(f =>
            {
                f.WithTopic("Tempo");
            }).WithTopicFilter(f =>
            {
                f.WithTopic($"Azienda/{args[0]}/Acqua");
            }
        ).Build();

    await mqttClient.SubscribeAsync(mqttSubscribeOptions);

    #endregion

    mqttClient.ApplicationMessageReceivedAsync += async e =>
    {
        /* Se il topic e' tempo allora aggiorno la data
           Altrimenti se e' dagli attuatori rigiro il messaggio alla rest api
           Stessa cosa per i sensori
           Invece in caso il topic e' irrigazione controllo i valori del messaggio e se e' necessario faccio partire gli attuatori
           Infine se e' acqua allora scrivo su console quanta acqua mi rimane
         */
        if (e.ApplicationMessage.Topic.Contains("Tempo"))
        {
            var text = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);
            data = DateTime.Parse(text);
        } else if (e.ApplicationMessage.Topic.Contains("Attuatori"))
        {
            var text = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"Azienda/{args[0]}/Evento")
                .WithPayload(text)
                .Build();
            await mqttClient.PublishAsync(applicationMessage);

        }
        else if (e.ApplicationMessage.Topic.Contains("Sensori"))
        {
            var text = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"Azienda/{args[0]}/Evento")
                .WithPayload(text)
                .Build();
            await mqttClient.PublishAsync(applicationMessage);
        } else if (e.ApplicationMessage.Topic.Contains("Irrigazione"))
        {
            var json = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);
            
            var evento = JsonConvert.DeserializeAnonymousType(json, new
            {
                campo = 0,
                umidita = .0,
                umidita_critica = .0,
                umidita_preferita = .0,
                tempo_irrigazione = 0f,
            });

            if (campiCritici.Contains(evento.campo))
                return;

            if (evento.umidita <= evento.umidita_preferita || evento.umidita <= evento.umidita_critica)
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"Cmd/Azienda/{args[0]}/Campo/{evento.campo}/Attuatori")
                    .WithPayload("1")
                    .Build();
                await mqttClient.PublishAsync(applicationMessage);

                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += Timer;

                worker.RunWorkerAsync($"{evento.campo}|{data.AddHours(evento.tempo_irrigazione)}");
            }
        } else if (e.ApplicationMessage.Topic.Contains("Acqua"))
        {
            var json = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);

            var evento = JsonConvert.DeserializeAnonymousType(json, new
            {
                acqua_rimanente = 0,
            });

            Console.WriteLine(evento.acqua_rimanente);
        }
    };

    while (true)
    {
    }

    await mqttClient.DisconnectAsync();
}



// Metodo che fa partire il timer per un certo periodo e quando finisce manda un messaggio di spegnimento all'attuatore
void Timer(Object sender, DoWorkEventArgs e)
{
    var info = ((string)e.Argument).Split('|');


    while (true)
    {
        if (data >= DateTime.Parse(info[1]))
            break;
        //Console.WriteLine($"Timer: {data}, Campo: {info[0]}");
    }

    var applicationMessage = new MqttApplicationMessageBuilder()
        .WithTopic($"Cmd/Azienda/{args[0]}/Campo/{info[0]}/Attuatori")
        .WithPayload("0")
        .Build();

    client.PublishAsync(applicationMessage);
}