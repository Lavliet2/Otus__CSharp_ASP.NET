using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.Shared.Events;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoCodeReceivedConsumer : IConsumer<PromoCodeReceivedEvent>
    {
        private readonly ILogger<PromoCodeReceivedConsumer> _logger;
        private readonly IPromoCodeService _promoCodeService;

        public PromoCodeReceivedConsumer(ILogger<PromoCodeReceivedConsumer> logger, IPromoCodeService promoCodeService)
        {
            _logger = logger;
            _promoCodeService = promoCodeService;
        }

        public async Task Consume(ConsumeContext<PromoCodeReceivedEvent> context)
        {
            _logger.LogInformation("Received promo code event: {PromoCode}", context.Message.PromoCode);

            await _promoCodeService.GivePromoCodeToCustomersAsync(context.Message);
        }
    }
}
