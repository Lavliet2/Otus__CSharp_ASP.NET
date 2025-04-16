using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Services;
using Pcf.Shared.Events;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PartnerCreatedConsumer : IConsumer<PromoCodeReceivedEvent>
    {
        private readonly ILogger<PartnerCreatedConsumer> _logger;
        private readonly IEmployeeService _employeeService;

        public PartnerCreatedConsumer(ILogger<PartnerCreatedConsumer> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        public async Task Consume(ConsumeContext<PromoCodeReceivedEvent> context)
        {
            _logger.LogInformation("Received promo code: {PromoCode}, ManagerId: {ManagerId}",
                context.Message.PromoCode, context.Message.ManagerId);

            if (context.Message.ManagerId.HasValue)
            {
                await _employeeService.UpdateAppliedPromocodesAsync(context.Message.ManagerId.Value);
            }
        }
    }
}
