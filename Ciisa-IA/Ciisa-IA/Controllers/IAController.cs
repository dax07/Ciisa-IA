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

        [HttpGet(Name = "GetAnswer")]
        public async Task<ActionResult<string>> GetAnswer([FromQuery] string request)
        {
            if (string.IsNullOrWhiteSpace(request))
            {
                return BadRequest("El parámetro 'request' es obligatorio.");
            }

            await Task.Delay(10); // Ejemplo de tarea asincrónica

            var response = $"Hola, me preguntaste esto ¿verdad? : {request}";
            return Ok(response);
        }
    }
}
