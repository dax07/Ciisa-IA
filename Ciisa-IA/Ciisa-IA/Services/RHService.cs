namespace Ciisa_IA.Services
{
    public class RHService
    {

        private readonly AIService AIService;

        private readonly string _baseContext = """
                                            Toma el papel de un reclutador de recursos humanos, el cuál se especializa en analizar documentos de tipo curriculum vitae
                                            y tomar desiciones estrategicas para contratar a nuevos candidatos de cualquier area empresarial, tienes que dar respuestas lo más
                                            resumidas posibles y con un valor de entendimiento que vaya al punto.
                                            """;

        public RHService()
        {
            AIService = new AIService();
        }

        public async Task<string> AnalyzeVacancy(string vacancyDescription)
        {
            string prompt = $"Tenemos la siguiente vacante: \n\n {vacancyDescription} \n\n podría darme los requerimientos base de esta vacante";

            string finalPrompt = _baseContext + prompt;

            return await AIService.SendPrompt(finalPrompt);  
        }
    }
}
