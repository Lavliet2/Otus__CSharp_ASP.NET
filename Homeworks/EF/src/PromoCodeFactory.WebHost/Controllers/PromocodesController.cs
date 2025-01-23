using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{

    /// <summary>
    /// Контроллер для работы с промокодами
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;

        public PromocodesController(IRepository<PromoCode> promoCodeRepository, IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _promoCodeRepository = promoCodeRepository;
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }


        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            var response = promoCodes.Select(p => new PromoCodeShortResponse
            {
                Id = p.Id,
                Code = p.Code,
                ServiceInfo = p.ServiceInfo,
                BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = p.PartnerName
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preferenceId = Guid.Parse(request.Preference);
            var preference = await _preferenceRepository.Query().FirstOrDefaultAsync(p => p.Id == preferenceId);
            if (preference == null)
            {
                return BadRequest("Invalid preference ID.");
            }

            var customers = await _customerRepository
                .Query()
                .Include(c => c.CustomerPreferences)
                .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == preferenceId))
                .ToListAsync();

            foreach (var customer in customers)
            {
                var promoCode = new PromoCode
                {
                    Id = Guid.NewGuid(),
                    Code = request.PromoCode,
                    ServiceInfo = request.ServiceInfo,
                    PartnerName = request.PartnerName,
                    BeginDate = DateTime.Now, 
                    EndDate = DateTime.Now.AddMonths(1), 
                    Preference = preference,
                    Customer = customer 
                };

                await _promoCodeRepository.AddAsync(promoCode);

                customer.PromoCodes.Add(promoCode);
                await _customerRepository.UpdateAsync(customer);
            }

            return Ok(new { message = "Промокод создан и выдан клиентам с указанным предпочтением." });
        }
    }
}