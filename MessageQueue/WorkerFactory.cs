using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace MessageQueue
{
    public class WorkerFactory : BackgroundService
    {
        private readonly List<Worker> workers = new();
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < 5; i++)
            {
                var worker = new Worker();
                workers.Add(worker);
                worker.ExecuteAsync(stoppingToken);
            }
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            foreach (Worker worker in workers)
            {
                worker.Dispose();
            }
        }
    }
}
