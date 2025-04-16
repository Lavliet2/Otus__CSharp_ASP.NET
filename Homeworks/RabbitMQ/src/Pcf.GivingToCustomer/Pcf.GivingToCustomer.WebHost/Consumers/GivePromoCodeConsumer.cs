using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Shared.Events;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoCodeReceivedConsumer : IConsumer<PromoCodeReceivedEvent>
    {
        private readonly ILogger<PromoCodeReceivedConsumer> _logger;

        public PromoCodeReceivedConsumer(ILogger<PromoCodeReceivedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PromoCodeReceivedEvent> context)
        {
            _logger.LogInformation("Giving promo code to customer: {CustomerId}", context.Message.ManagerId);
            // TODO: логика выдачи промокода клиенту
            return Task.CompletedTask;
        }
    }
}
