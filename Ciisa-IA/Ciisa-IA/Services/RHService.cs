using Ciisa_IA.Dtos;
using Ciisa_IA.Helpers;
using OpenAI.Chat;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            List<CompetencyDistribution> competencyDistribution = CompetencyDistributionData.All
                //.Where(x => x.JobLevel == profileCissaDTO.JobLevel)
                .ToList();

            // Filtrar las competencias necesarias
            //List<string> filterComptencyCategories = competencyDistribution.CompetencyCategories
            //    .Where(c => c.Value > 0)
            //    .Select(c => c.Key)
            //    .ToList();

            List<Competency> competencies = CompetenciesData.All
                //.Where(x => filterComptencyCategories.Contains(x.CategoryName))
                .ToList();

            StringBuilder competenciesText = new();
            StringBuilder jobsLevelText = new();

            foreach (Competency competency in competencies)
            {
                string competencyItemText = $"""
                 **
                 {competency.Id}.- {competency.Name}, 
                 Categoría: {competency.CategoryName},
                 Descripción: {competency.Description}
                 **
                 """;

                competenciesText.AppendLine(competencyItemText);
            }

            foreach (CompetencyDistribution competency in competencyDistribution)
            {
                var textoCategorias = string.Join(", ",
                                            competency.CompetencyCategories
                                            .Select(kvp => $"{kvp.Key}: {kvp.Value}")
);

                string competencyItemText = $"""
                 **
                 Nivel de organizacional: {competency.JobLevel}, 
                 Categorías y sus numeros de competencias requeridas: {textoCategorias}
                 **
                 """;

                jobsLevelText.AppendLine(competencyItemText);
            }

            //StringBuilder competencyDistributionText = new();

            //foreach (var category in competencyDistribution.CompetencyCategories.Where(c => c.Value > 0))
            //{
            //    competencyDistributionText.AppendLine($"- **Categoría: {category.Key}**: Selecciona solo y exactamente **{category.Value} competencias**");
            //}

            //CompetencyLevelRange competencyLevelRange = CompetencyRangesData.All
            //    .Where(x => x.JobLevel == profileCissaDTO.JobLevel)
            //    .First();

            //StringBuilder requirementsText = new();

            //foreach (var requirement in competencyLevelRange.Requirements)
            //{
            //    if (requirement.ExpectedCount > 0)
            //    {
            //        if (requirement.ExpectedCount > 1)
            //            requirementsText.AppendLine($"- {requirement.ExpectedCount} competencias de la lista debén estar ponderadas en **{requirement.Score}**");
            //        else
            //            requirementsText.AppendLine($"- {requirement.ExpectedCount} competencia de la lista debé estar ponderada en **{requirement.Score}**");
            //    }

            //}

            const string jsonTemplate = """
                "nivelOrganizacional": "{nivel organizacional recomendado}",
                "competencias": [
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

            string systemPrompt = $$"""
                Lista de competencias y sus categorías:
                | #  | Competencia                 | Categoría       | Descripción                                                                                                                       |
                | -- | --------------------------- | --------------- | --------------------------------------------------------------------------------------------------------------------------------- |
                | 1  | Pensamiento Estratégico     | Estratégicas    | Habilidad para desarrollar estrategias y adecuarlas a los cambios del entorno, detectando nuevas oportunidades y mejor desempeño. |
                | 2  | Análisis de Problemas       | Funcionales     | Obtener información relevante e identificar elementos críticos para elegir acciones apropiadas.                                   |
                | 3  | Apertura al Cambio          | Funcionales     | Nivel de apertura para comprender, aceptar y manejar nuevas ideas o enfoques en diferentes entornos.                              |
                | 4  | Aprendizaje                 | Operativas      | Capacidad para adquirir y aplicar nuevos conocimientos y habilidades de manera constante.                                         |
                | 5  | Autoconfianza               | Intrapersonales | Capacidad de confiar en las propias habilidades, decisiones y criterios para actuar con seguridad.                                |
                | 6  | Autodesarrollo              | Intrapersonales | Disposición y esfuerzo por mejorar continuamente en lo personal y profesional.                                                    |
                | 7  | Comunicación Oral           | Sociales        | Expresar ideas de forma clara, precisa y coherente al comunicarse con otros.                                                      |
                | 8  | Control de Actividades      | Operativas      | Capacidad para monitorear y verificar el cumplimiento de tareas y responsabilidades.                                              |
                | 9  | Delegación                  | Funcionales     | Habilidad para asignar tareas a otros, manteniendo supervisión y seguimiento adecuados.                                           |
                | 10 | Desarrollo del Talento      | Funcionales     | Capacidad de identificar, motivar y fortalecer el potencial del equipo.                                                           |
                | 11 | Dinamismo                   | Operativas      | Actuar con energía, entusiasmo y proactividad para alcanzar los objetivos.                                                        |
                | 12 | Dominio del Estrés          | Intrapersonales | Capacidad para mantener el autocontrol ante situaciones de presión o tensión.                                                     |
                | 13 | Empowerment                 | Sociales        | Habilidad para otorgar confianza, autonomía y responsabilidad al equipo.                                                          |
                | 14 | Enfoque a Resultados        | Funcionales     | Compromiso con el logro de metas establecidas, enfocado en efectividad y eficiencia.                                              |
                | 15 | Enfoque a la Calidad        | Operativas      | Buscar excelencia en procesos y resultados, respetando normas y estándares.                                                       |
                | 16 | Enfoque a Ventas            | Sociales        | Orientación al cliente y generación de oportunidades de negocio mediante la venta.                                                |
                | 17 | Liderazgo                   | Funcionales     | Capacidad para guiar, motivar y coordinar personas hacia el cumplimiento de objetivos.                                            |
                | 18 | Manejo de Conflictos        | Sociales        | Identificar y resolver desacuerdos o tensiones de forma constructiva y eficaz.                                                    |
                | 19 | Madurez Social              | Intrapersonales | Comportamiento ético y equilibrado en la relación con otros.                                                                      |
                | 20 | Negociación                 | Sociales        | Capacidad para llegar a acuerdos mutuamente beneficiosos en conflictos o intereses comunes.                                       |
                | 21 | Iniciativa                  | Funcionales     | Actuar sin necesidad de indicaciones, proponiendo mejoras o soluciones.                                                           |
                | 22 | Innovación                  | Funcionales     | Generar e implementar ideas nuevas y eficaces en el entorno de trabajo.                                                           |
                | 23 | Organización                | Funcionales     | Distribuir el tiempo y recursos adecuadamente para cumplir objetivos.                                                             |
                | 24 | Orientación al Servicio     | Operativas      | Disposición y actitud de ayuda hacia clientes internos y externos.                                                                |
                | 25 | Pensamiento Estratégico     | Estratégicas    | Capacidad de visualizar escenarios futuros y tomar decisiones alineadas con la misión y visión de la organización.                |
                | 26 | Perseverancia               | Operativas      | Mantenerse constante ante dificultades o fracasos, buscando cumplir objetivos.                                                    |
                | 27 | Persuasión                  | Sociales        | Convencer a otros a través de argumentos sólidos y comunicación eficaz.                                                           |
                | 28 | Planeación                  | Funcionales     | Definir objetivos y establecer los pasos necesarios para lograrlos.                                                               |
                | 29 | Relaciones Interpersonales  | Sociales        | Establecer relaciones de colaboración y respeto con otras personas.                                                               |
                | 30 | Resolución de Problemas     | Funcionales     | Identificar, analizar y resolver situaciones complejas de forma efectiva.                                                         |
                | 31 | Responsabilidad             | Intrapersonales | Cumplir con los compromisos adquiridos y asumir las consecuencias de los propios actos.                                           |
                | 32 | Sensibilidad a Lineamientos | Operativas      | Actuar conforme a normas, políticas y procedimientos establecidos.                                                                |
                | 33 | Team Building               | Sociales        | Fomentar el trabajo colaborativo y fortalecer la cohesión del equipo.                                                             |
                | 34 | Toma de Decisiones          | Funcionales     | Elegir la mejor opción entre diversas alternativas para alcanzar un objetivo.                                                     |
                | 35 | Trabajo en Equipo           | Operativas      | Colaborar y contribuir con otros para alcanzar objetivos comunes.  

                Se te compartira el puesto y las responsabilidades. A continuacion realiza lo siguientes pasos

                Eres un reclutador de Recursos Humanos experto en análisis de perfiles laborales.  
                Tu tarea es seleccionar competencias de una lista maestra y asignarles puntuaciones específicas según reglas estrictas.  
                Debes generar un JSON estructurado con las competencias elegidas, categorizadas y puntuadas.

                ---

                ## 1. Selecciona el nivel organizacional más adecuado

                De acuerdo con las responsabilidades del puesto, selecciona **solo uno** de los siguientes niveles y **aplica exactamente** sus reglas:

                ### Distribución por nivel organizacional:

                **Nivel Estratégico**
                - Categorías: 1 Estratégica, 8 Funcionales, 1 Operativa, 2 Sociales  
                - Ponderaciones: 2 con valor 3, 2 con 3.5, 4 con 4, 2 con 4.5, 2 con 5  
                - Total: 12 competencias

                **Nivel Funcional**
                - Categorías: 0 Estratégicas, 9 Funcionales, 1 Operativa, 2 Sociales  
                - Ponderaciones: 2 con valor 3, 2 con 3.5, 5 con 4, 2 con 4.5, 1 con 5  
                - Total: 12 competencias

                **Nivel Coordinador**
                - Categorías: 0 Estratégicas, 5 Funcionales, 4 Operativas, 3 Sociales  
                - Ponderaciones: 2 con valor 3, 4 con 3.5, 4 con 4, 2 con 4.5, 0 con 5  
                - Total: 12 competencias

                **Nivel Ejecutor**
                - Categorías: 0 Estratégicas, 4 Funcionales, 5 Operativas, 3 Sociales  
                - Ponderaciones: 3 con valor 3, 4 con 3.5, 4 con 4, 1 con 4.5, 0 con 5  
                - Total: 12 competencias

                ---

                ## 2. Selecciona las competencias exactas

                De la lista maestra de competencias (proporcionada más abajo), selecciona **exactamente** la cantidad requerida por categoría para el nivel elegido.

                ⚠️ No uses más ni menos de lo requerido.  
                ⚠️ No inventes competencias ni cambies categorías.  
                ⚠️ No repitas competencias.

                ✅ Ejemplo válido para "Ejecutor":
                - Funcionales: 4 competencias
                - Operativas: 5 competencias
                - Sociales: 3 competencias

                ❌ Ejemplo inválido:
                - Funcionales: 5
                - Sociales: 4

                ---

                ## 3. Asigna puntuaciones exactamente como se indica

                Distribuye las puntuaciones **solo entre las competencias seleccionadas en el paso anterior**.  

                Antes de asignar las puntuaciones, realiza esta validación previa:

                ### Conteo previo:

                - ¿Tienes exactamente 12 competencias?
                - ¿Se respetan las categorías por nivel?
                - ¿Cuántas llevarán puntuación 3? ¿3.5? ¿4? ¿4.5? ¿5?
                  - Por ejemplo, para "Ejecutor":
                    - 3 con 3
                    - 4 con 3.5
                    - 4 con 4
                    - 1 con 4.5
                    - 0 con 5

                ✅ Solo si el conteo exacto coincide con las reglas, procede.

                ❌ Si no puedes cumplirlo exactamente, responde con:
                ```json
                { "error": "No se pudo cumplir con la distribución exacta por puntuación y categoría." }


            
            """;
            //string prompt = $$"""
            //    Tienes la siguiente información de un perfil:
            //    - **Nombre de puesto: Reclutador,
            //    - **Responsabilidades: Publicar y difundir vacantes en los distintos medios autorizados.,
            //    Realizar información curricular conforme al perfil solicitado. Aplicar entrevistas iniciales, psicométricas y técnicas cuando corresponda.
            //    Coordinar entrevistas con líderes o responsables del área solicitante.
            //    Llevar control y seguimiento de cada proceso de reclutamiento en curso.
            //    Documentar resultados en el sistema de gestión de talento.
            //    Apoyar en eventos de atracción como ferias de empleo o convenios universitarios.
            //    Garantizar una experiencia positiva para el candidato durante todo el proceso.
            //    Cumplir con los tiempos de cobertura y métricas establecidas para su puesto.
            //    Participar en sesiones de alineación con el área de Recursos Humanos.
            // """;
            //string prompt = $$"""
            //    Nombre de puesto: Desarrollador Senior,
            //    Responsabilidades: Desarrollar código, resolver problemas técnicos, implementar soluciones y realizar pruebas unitarias
            // """;

            string prompt = $"""
                 **Nombre de puesto: {profileCissaDTO.JobTitle}
                 **Responsabilidades: {profileCissaDTO.Responsibilities}
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
