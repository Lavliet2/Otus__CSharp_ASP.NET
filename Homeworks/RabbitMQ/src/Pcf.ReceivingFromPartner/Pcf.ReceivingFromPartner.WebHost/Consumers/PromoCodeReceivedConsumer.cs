using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Shared.Events;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.WebHost.Consumers
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
            _logger.LogInformation("Partner service received promo code: {PromoCode}", context.Message.PromoCode);
            // TODO: на будущее
            return Task.CompletedTask;
        }
    }
}
