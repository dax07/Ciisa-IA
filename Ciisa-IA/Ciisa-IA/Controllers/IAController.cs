using Ciisa_IA.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Ciisa_IA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IAController : ControllerBase
    {
        private readonly ILogger<IAController> _logger;

        public IAController(ILogger<IAController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostAnswer")]
        public async Task<ActionResult<string>> GetAnswer([FromBody] RequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Request))
            {
                return BadRequest("La propiedad 'request' es obligatoria.");
            }

            await Task.Delay(10); // Simulación de trabajo asincrónico

            var response = $"Hola, me preguntaste esto ¿verdad? : {dto.Request}";
            return Ok(response);
        }
    }
}
