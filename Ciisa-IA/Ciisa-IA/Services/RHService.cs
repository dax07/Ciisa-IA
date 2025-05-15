using Ciisa_IA.Dtos;
using Ciisa_IA.Helpers;
using OpenAI.Chat;
using System.Text;

namespace Ciisa_IA.Services
{
    public class RHService
    {

        private readonly AIService _AIService;

        private readonly string _baseContext = """
                                            Toma el papel de un reclutador de recursos humanos, el cuál es un experto en redacción de perfiles de reclutamiento 
                                            para vacantes de trabajo empresariales, tienes que dar respuestas lo más resumidas posibles y con un valor de entendimiento que vaya al punto.
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

        public async Task<string> CreateProfile(ProfileCissaDTO profileCissaDTO)
        {
            CompetencyDistribution competencyDistribution = CompetencyDistributionData.All
                .Where(x => x.JobLevel == profileCissaDTO.JobLevel)
                .First();

            // Filtrar las competencias necesarias
            List<string> filterComptencyCategories = competencyDistribution.CompetencyCategories
                .Where(c => c.Value > 0)
                .Select(c => c.Key)
                .ToList();

            List<Competency> competencies = CompetenciesData.All
                .Where(x => filterComptencyCategories.Contains(x.CategoryName))
                .ToList();

            StringBuilder competenciesText = new();

            foreach (Competency competency in competencies)
            {
                string competencyItemText = $"""
                 { competency.Id}.- {competency.Name} 
                 Categoría: {competency.CategoryName}
                 Descripción: {competency.Description}
                 """;

                competenciesText.AppendLine(competencyItemText);
            }

            StringBuilder competencyDistributionText = new();

            foreach (var category in competencyDistribution.CompetencyCategories.Where(c => c.Value > 0))
            {
                competencyDistributionText.AppendLine($"- **{category.Key}**: Selecciona exactamente **{category.Value} competencias** de esta categoría");
            }

            CompetencyLevelRange competencyLevelRange = CompetencyRangesData.All
                .Where(x => x.JobLevel == profileCissaDTO.JobLevel)
                .First();

            StringBuilder requirementsText = new();

            foreach (var requirement in competencyLevelRange.Requirements)
            {
                if (requirement.ExpectedCount > 0)
                {
                    if (requirement.ExpectedCount > 1)
                        requirementsText.AppendLine($"- {requirement.ExpectedCount} competencias de la lista debén estar ponderadas en **{requirement.Score}**");
                    else
                        requirementsText.AppendLine($"- {requirement.ExpectedCount} competencia de la lista debé estar ponderada en **{requirement.Score}**");
                }
                
            }

            const string jsonTemplate = """
                [
                    {
                    "name": "{0}",
                    "score": {1}
                    },
                    {
                    "name": "{2}",
                    "score": {3}
                    }
                ]
                """;

            string systemPrompt = $"""
                Eres un reclutador de RR.HH. experto en redactar perfiles. 
                Sigue **estrictamente** estas reglas de distribución y ponderaciones:
                {requirementsText}
                Usa **solo** la lista de competencias que te doy y no inventes otras.
                Responde únicamente con las competencias y sus ponderaciones.
            """;

            string prompt = $"""
             Tienes la siguiente información de un perfil:
             - Nombre de puesto: {profileCissaDTO.JobTitle}
             - Nivel de puesto: {profileCissaDTO.JobLevel}
             - Responsabilidades: {profileCissaDTO.Responsibilities}

             Instrucciones:
             1. En base a la siguiente lista:
             {competenciesText}
             debes elegir {competencyLevelRange.TotalRequirements} competencias de la siguiente manera:
             {competencyDistributionText}

             4. **Salida JSON** por favor descarta cualquier otra palabra que no sea la estructura del json:

             {jsonTemplate}
            
             """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(prompt)
            };

            string response = await _AIService.SendPrompt(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 4096,
                Temperature = 0.0f,
                TopP = 1.0f
            });

            return response;
        }

    }
}
