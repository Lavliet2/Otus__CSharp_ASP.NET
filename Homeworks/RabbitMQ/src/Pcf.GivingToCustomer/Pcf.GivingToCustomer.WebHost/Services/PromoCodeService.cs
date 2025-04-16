using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.Shared.Events;

namespace Pcf.GivingToCustomer.WebHost.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IRepository<Customer> _customersRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly ILogger<PromoCodeService> _logger;

        public PromoCodeService(
            IRepository<Customer> customersRepository,
            IRepository<Preference> preferencesRepository,
            IRepository<PromoCode> promoCodesRepository,
            ILogger<PromoCodeService> logger)
        {
            _customersRepository = customersRepository;
            _preferencesRepository = preferencesRepository;
            _promoCodesRepository = promoCodesRepository;
            _logger = logger;
        }

        public async Task GivePromoCodeToCustomersAsync(PromoCodeReceivedEvent message)
        {
            _logger.LogInformation("Start processing promo code from partner {PartnerName}", message.PartnerName);

            var preference = (await _preferencesRepository
                .GetWhere(p => p.Name == "default"))
                .FirstOrDefault();

            if (preference == null)
            {
                _logger.LogWarning("No matching preference found");
                return;
            }

            var customers = await _customersRepository
                .GetWhere(c => c.Preferences.Any(p => p.PreferenceId == preference.Id));

            var promoCode = new PromoCode
            {
                Id = Guid.NewGuid(),
                Code = message.PromoCode,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                PartnerId = Guid.NewGuid(),
                ServiceInfo = $"Промокод от {message.PartnerName}",
                Preference = preference,
                PreferenceId = preference.Id,
                Customers = customers.Select(c => new PromoCodeCustomer
                {
                    CustomerId = c.Id
                }).ToList()
            };

            await _promoCodesRepository.AddAsync(promoCode);

            _logger.LogInformation("Promo code saved and assigned to {Count} customers", promoCode.Customers.Count);
        }
    }
}
