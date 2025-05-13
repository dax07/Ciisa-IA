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
        private readonly CVService _cvService;

        public IAController(ILogger<IAController> logger, CVService cvService)
        {
            _logger = logger;
            _cvService = cvService;
        }

        [HttpPost(Name = "PostAnswer")]
        public async Task<ActionResult<string>> GetAnswer([FromBody] RequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Request))
            {
                return BadRequest("La propiedad 'request' es obligatoria.");
            }

            await Task.Delay(10); // Simulación de trabajo asincrónico

            string response = string.Empty;

            if( string.IsNullOrEmpty(dto.ConversationId) )
            {
                response = await _cvService.SendPrompt(dto.Request);
            }
            else
            {
                response = await _cvService.ContinuePrompt(dto.Request, dto.ConversationId);
            }


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

        //const formData = new FormData();
        //formData.append("file", fileInput.files[0]);

        //fetch("/api/upload", {
        //        method: "POST",
        //    body: formData
        //});

    }
}
