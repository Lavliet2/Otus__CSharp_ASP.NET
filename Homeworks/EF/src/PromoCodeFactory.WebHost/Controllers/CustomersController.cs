using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Контроллер для работы с клиентами
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<PromoCode> _promoCodeRepository;

        public CustomersController(IRepository<Customer> customerRepository, IRepository<PromoCode> promoCodeRepository)
        {
            _customerRepository = customerRepository;
            _promoCodeRepository = promoCodeRepository;
        }

        /// <summary>
        /// Получить всех клиентов
        /// </summary>
        /// <returns>Список клиентов</returns>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            var response = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить клиента по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Клиент</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository
                .Query()
                .Include(c => c.PromoCodes)
                .Include(c => c.CustomerPreferences)
                    .ThenInclude(cp => cp.Preference)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            var response = new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Preferences = customer.CustomerPreferences.Select(cp => new PreferenceResponse
                {
                    Id = cp.Preference.Id,
                    Name = cp.Preference.Name
                }).ToList(),
                PromoCodes = customer.PromoCodes.Select(p => new PromoCodeShortResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    ServiceInfo = p.ServiceInfo,
                    BeginDate = p.BeginDate.ToString("yyyy-MM-dd"),
                    EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                    PartnerName = p.PartnerName
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Создать клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds.Select(id => new CustomerPreference { PreferenceId = id }).ToList()
            };

            await _customerRepository.AddAsync(customer);
            //return CreatedAtAction(nameof(GetCustomerAsync), new { id = customer.Id }, customer);
            // Не понял как правильно ответ с новым customer отдать
            return Ok(GetCustomerAsync(customer.Id));
        }

        /// <summary>
        /// Редактировать клиента
        /// </summary>
        /// <param name="id">Идентификатору</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository
                .Query()
                .Include(c => c.CustomerPreferences)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.CustomerPreferences = request.PreferenceIds.Select(id => new CustomerPreference { CustomerId = customer.Id, PreferenceId = id }).ToList();

            await _customerRepository.UpdateAsync(customer);

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository
                .Query()
                .Include(c => c.PromoCodes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            foreach (var promoCode in customer.PromoCodes)
            {
                await _promoCodeRepository.DeleteAsync(promoCode.Id);
            }

            await _customerRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}