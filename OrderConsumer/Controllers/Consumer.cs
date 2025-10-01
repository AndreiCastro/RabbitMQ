using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Model;
using System;
using System.Threading.Tasks;

namespace OrderConsumer.Controllers
{
    public class Consumer : IConsumer<Ticket>
    {
        private readonly ILogger<Consumer> logger;

        public Consumer(ILogger<Consumer> logger)
        {
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<Ticket> context)
        {
            await Console.Out.WriteLineAsync(context.Message.UserName);

            logger.LogInformation($"Nova mensagem recebida: {context.Message.UserName} {context.Message.Location}");
        }
    }
}
