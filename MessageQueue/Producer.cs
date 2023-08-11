using MessageQueue.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageQueue
{
    public class Producer
    {
        public void SendMessage(FileMessage fileMessage)
        {
            var message = JsonSerializer.Serialize(fileMessage);
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "FileQueue",
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: "FileQueue",
                           basicProperties: null,
                           body: body);
        }
    }
}
