using MessageQueue.Models;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System.Reflection;

public class Producer
{

    public void SendMessage(FileMessage fileMessage)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("filecopywithlogs", ExchangeType.Fanout);

        var dir = new DirectoryInfo(fileMessage.OriginLocation);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        foreach (FileInfo file in dir.GetFiles())
        {
            fileMessage.FileName = file.Name;
            var message = JsonSerializer.Serialize(fileMessage);

            var body = Encoding.UTF8.GetBytes(message);

            var props = channel.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Expiration = "1000";

            channel.BasicPublish(exchange: "filecopywithlogs",
                           routingKey: "",
                           basicProperties: null,
                           body: body);
        }
        
    }
}

