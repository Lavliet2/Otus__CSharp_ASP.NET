using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pcf.Preference.Core.Models;
using Pcf.Preference.DataAccess;

namespace Pcf.Preference.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PreferenceController : ControllerBase
    {
        private readonly RedisPreferenceRepository _repository;

        public PreferenceController(RedisPreferenceRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _repository.GetPreferencesAsync());

        /// <summary>
        /// Создать список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Set([FromBody] List<UserPreference> preferences)
        {
            await _repository.SetPreferencesAsync(preferences);
            return Ok();
        }
    }
}
