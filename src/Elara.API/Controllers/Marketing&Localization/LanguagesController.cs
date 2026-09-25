using Elara.Application.DTOs;
using Elara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elara.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public LanguagesController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            var result = await _localizationService.GetLanguagesAsync();
            return Ok(result);
        }

        [HttpPost("languages")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageRequestDto request)
        {
            var result = await _localizationService.CreateLanguageAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("languages/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLanguage(int id, [FromBody] UpdateLanguageRequestDto request)
        {
            var result = await _localizationService.UpdateLanguageAsync(id, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("resource-strings")]
        public async Task<IActionResult> GetResourceStrings([FromQuery] string languageCode)
        {
            var result = await _localizationService.GetResourceStringsAsync(languageCode);
            return Ok(result);
        }

        [HttpPost("resource-strings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpsertResourceString([FromBody] UpsertResourceStringRequestDto request)
        {
            var result = await _localizationService.UpsertResourceStringAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("resource-strings/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteResourceString(int id)
        {
            var result = await _localizationService.DeleteResourceStringAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}