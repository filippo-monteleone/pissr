using System.Text;
using MQTTnet;
using MQTTnet.Client;

namespace AttuatoreMqtt;

/*
 * Questa e' la classe dell'attuatore che ha il compito di connettersi al broker mqtt.
 * I dati gli vengono passati attraverso i metodi che espone.
 */
public class Attuatore
{

    public string State = "Off";
    public string Azienda = "test";
    public string Campo = "";
    public string Id = "";
    public IMqttClient mqttClient;

    public Attuatore()
    {
        var mqttFactory = new MqttFactory();
        mqttClient = mqttFactory.CreateMqttClient();
    }

    // Metodo usato per iniziare la comunicazione con il broker
    public async void Start()
    {
        var mqttFactory = new MqttFactory();

        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            
            // Setto lo stato dell'attuatore a seconda del messaggio arrivato
            switch (payload)
            {
                case "0":
                    State = "Off";
                    break;
                case "1":
                    State = "On";
                    break;
            }


            // Se e' un comando allora invio indietro al gestore l'ack
            if (e.ApplicationMessage.Topic.Contains("Cmd"))
            {

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"Azienda/{Azienda}/Campo/{Campo}/Attuatori")
                    .WithPayload("{" + $"Tipo: {(State == "Off" ? 5 : 4)}, Valore: null, DispositivoId: {Id}" + "}")
                    .Build();
                await mqttClient.PublishAsync(applicationMessage);
            }
        };

        await mqttClient.ConnectAsync(mqttClientOptions);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(
            f =>
            {
                f.WithTopic($"Cmd/Azienda/{Azienda}/Campo/{Campo}/Attuatori");
            }).Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions);
    }

    public async Task Disconnect()
    {
        await mqttClient.DisconnectAsync();
    }
}