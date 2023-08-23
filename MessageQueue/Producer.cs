using MessageQueue.Models;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

public class Producer
{
    public void SendMessage(FileMessage fileMessage)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "FileQueue",
                       durable: false,
                       exclusive: false,
                       autoDelete: false,
                       arguments: null);

        var dir = new DirectoryInfo(fileMessage.OriginLocation);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        foreach (FileInfo file in dir.GetFiles())
        {
            fileMessage.FileName = file.Name;
            var message = JsonSerializer.Serialize(fileMessage);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                           routingKey: "FileQueue",
                           basicProperties: null,
                           body: body);
        }
    }
}