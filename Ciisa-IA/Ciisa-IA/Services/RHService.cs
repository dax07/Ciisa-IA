using Ciisa_IA.Dtos;
using Ciisa_IA.Helpers;
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
                 {competency.Id}.- {competency.Name} 
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
                requirementsText.AppendLine($"- {requirement.ExpectedCount} competencias deben tener la ponderación **{requirement.Score}**");
            }

            string prompt = $"""
             Tienes la siguiente información de un perfil:

             - Nombre de puesto: {profileCissaDTO.JobTitle}
             - Nivel de puesto: {profileCissaDTO.JobLevel}
             - Responsabilidades: {profileCissaDTO.Responsibilities}

             **Instrucciones claras:**
             1. **Distribución por categorías**
             {competencyDistributionText.ToString()}
             - **IMPORTANTE**: Respeta el número exacto de competencias por categoría. No selecciones más ni menos de lo solicitado en cada categoría.
             

             2. **Ponderaciones**:
             Las competencias seleccionadas deben ser evaluadas con las siguientes ponderaciones, sin importar la categoría:
             {requirementsText.ToString()}
             **IMPORTANTE**: Asegúrate de que las competencias seleccionadas tengan la ponderación exacta indicada. 

             3. **Basado en las responsabilidades del puesto, el nombre del cargo, y las instrucciones anteriores**, selecciona las competencias de esta lista:
             
             {competenciesText.ToString()}

             4. **Formato de la respuesta**:
             - Categoría | Nombre Competencia | Ponderación
             **IMPORTANTE**: Respeta estrictamente el número de competencias por categoría y la distribución de ponderaciones como se indica. No se debe seleccionar más ni menos de lo solicitado en cada categoría.
             """;

            string finalPrompt = $"""
             {_baseContext} 
             {prompt} 
         """;

            return await _AIService.SendPrompt(finalPrompt);
        }

    }
}
