using Ciisa_IA.Dtos;

namespace Ciisa_IA.Services
{
    public class RHService
    {

        private readonly AIService _AIService;

        private readonly string _baseContext = """
                                            Toma el papel de un reclutador de recursos humanos, 
                                            el cuál es un experto en redacción de perfiles de reclutamiento 
                                            para vacantes de trabajo empresariales, tienes que dar respuestas lo más
                                            resumidas posibles y con un valor de entendimiento que vaya al punto.
                                            """;

        public RHService(AIService AIService)
        {
            _AIService = AIService;
        }

        public async Task<string> CreateProfile(ProfileRequirementsDTO profileRequirementsDTO)
        {
            string prompt = $"""
                En base a los siguientes datos: 

                Nombre de la vacante: {profileRequirementsDTO.ProfileName}  
                Experiencia: {profileRequirementsDTO.Experiencie}  
                Idiomas: {profileRequirementsDTO.Languages}
                Ubicación: {profileRequirementsDTO.Location}
                Tipo de contrato: {profileRequirementsDTO.TypeContract}
                Nivel de estudios: {profileRequirementsDTO.EducationLevel}
                 
                Crea un perfil de reclutamiento con máximo 500 palabras, por favor sientete libre de
                colocar tus datos de la manera que sea más organizada.
                """;

            string finalPrompt = _baseContext + prompt;

            return await _AIService.SendPrompt(finalPrompt);  
        }
    }
}
