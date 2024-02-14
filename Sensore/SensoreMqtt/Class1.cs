using System.Runtime.InteropServices.ComTypes;
using MQTTnet;
using MQTTnet.Client;
using System.Text;

namespace SensoreMqtt
{
    // Questa e' la classe del sensore che gestisce i messaggi mqtt e la logica del sensore.
    public class Sensore
    {
        public string Azienda = "test";
        public string Campo = "";
        public string Id = "";
        public IMqttClient mqttClient;

        public Sensore()
        {
            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
        }

        public async void Start()
        {
            var mqttFactory = new MqttFactory();

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                return Task.CompletedTask;
            };

            await mqttClient.ConnectAsync(mqttClientOptions);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(
                f =>
                {
                    f.WithTopic($"Azienda/{Azienda}/Campo/{Campo}/Sensori");
                }).Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions);

        }

        // Mando le misure al gestore iot
        public async Task SendData((int, double, int) data)
        {
            if (mqttClient.IsConnected)
            {
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"Azienda/{Azienda}/Campo/{Campo}/Sensori")
                    .WithPayload("{"+$"Tipo: {data.Item1}, Valore: {data.Item2}, DispositivoId: {data.Item3}"+"}")
                    .Build();
                await mqttClient.PublishAsync(applicationMessage);
            }

        }

        public async Task Disconnect()
        {
            await mqttClient.DisconnectAsync();
        }

    }
}
