using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using MessageQueue.Models;
using System.Text.Json;
using System.Data.Common;

namespace MessageQueue
{
    public class Worker : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly int _workerNumber;

        public Worker(int number)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "filecopywithlogs", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                              exchange: "filecopywithlogs",
                              routingKey: string.Empty);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            _workerNumber = number;
        }

        public Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<FileMessage>(content);

                var random = new Random();
                var number = random.Next(1, 100);
                if (number%2 == 0)
                {
                    throw new Exception();
                }
                 
                File.Copy(Path.Combine(message.OriginLocation, message.FileName), Path.Combine(message.NewLocation, message.FileName), true);
                Console.WriteLine($"File {message.FileName} was copied by {_workerNumber} worker");
            };

            _channel.BasicConsume(queue: _queueName,
                     autoAck: true,
                     consumer: consumer);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            Dispose();
        }
    }
}
