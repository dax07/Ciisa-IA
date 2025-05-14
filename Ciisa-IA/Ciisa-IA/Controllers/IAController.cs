using Ciisa_IA.Dtos;
using Ciisa_IA.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ciisa_IA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IAController : ControllerBase
    {
        private readonly ILogger<IAController> _logger;
        private readonly AIService _AIService;
        private readonly CVService _cvService;
        private readonly RHService _rhService;

        public IAController(ILogger<IAController> logger, AIService AIService,
            CVService cvService, RHService rhService)
        {
            _logger = logger;
            _AIService = AIService;
            _cvService = cvService;
            _rhService = rhService;
        }

        [HttpPost(Name = "PostAnswer")]
        public async Task<ActionResult<string>> GetAnswer([FromBody] RequestDto requestDTO)
        {
            string AIResponse = await _AIService.SendPrompt(requestDTO.Request);
            return Ok(AIResponse);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no v�lido");

            //var path = Path.Combine("Uploads", file.FileName);

            //using (var stream = new FileStream(path, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            return Ok("Archivo subido correctamente");
        }


        //[HttpPost("CreateProfile")]
        //public async Task<IActionResult> CreateProfile(ProfileRequirementsDTO profileRequirementsDTO)
        //{
        //    string AIResponse =  await _rhService.CreateProfile(profileRequirementsDTO);
        //    return Ok(AIResponse);
        //}

        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateProfile(ProfileCissaDTO profileCissaDTO)
        {
            string AIResponse = await _rhService.CreateProfile(profileCissaDTO);
            return Ok(AIResponse);
        }


        [HttpPost("ProccessCV")]
        public async Task<IActionResult> GetProcessData([FromBody] RequestDto dto)
        {
            // Inicializamos la respuesta
            string AIResponse = string.Empty;

            // Evaluamos si es una continuacion de la conversación
            if ( string.IsNullOrEmpty(dto.ConversationId) )
            {
                AIResponse = await _cvService.SendPrompt(dto.Request);
            }
            else
            {
                AIResponse = await _cvService.ContinueConversationAsync(dto.ConversationId, dto.Request);
            }

            return Ok(AIResponse);
        }
    }


}
