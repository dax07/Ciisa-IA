using Ciisa_IA.Dtos;
using Ciisa_IA.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Ciisa_IA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IAController : ControllerBase
    {
        private readonly ILogger<IAController> _logger;
        private readonly AIService AIService;

        public IAController(ILogger<IAController> logger)
        {
            _logger = logger;
            AIService = new AIService();
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido");

            //var path = Path.Combine("Uploads", file.FileName);

            //using (var stream = new FileStream(path, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            return Ok("Archivo subido correctamente");
        }


        [HttpPost]
        public async Task<IActionResult> Post(PromptDTO promptDTO)
        {
            string AIResponse = await AIService.SendPrompt(promptDTO.Context);
            return Ok(AIResponse);
        }

    }


}
