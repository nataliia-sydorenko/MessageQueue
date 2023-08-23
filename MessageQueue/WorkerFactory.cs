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
                var worker = new Worker(i);
                workers.Add(worker);
                worker.ExecuteAsync(stoppingToken);
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (Worker worker in workers)
            {
                worker.Dispose();
            }

            return base.StopAsync(cancellationToken);
        }
    }
}
