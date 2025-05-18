using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System;

namespace Ciisa_IA.Services
{
    public class CVService
    {
        private readonly AIService AIService;
        private readonly IMemoryCache _cache;

        public CVService(IMemoryCache cache)
        {
            AIService = new AIService();
            _cache = cache;
        }

        public async Task<string> SendPrompt(string prompt)
        {
            // Crear nuevo ID de conversacion
            string conversationId = Guid.NewGuid().ToString("N");

            // Definimos el mensaje de sistema con **todas** las reglas y estructura
            var systemInstruction = @"
        Eres un Analista de Currículums Vitae. Cuando recibas a continuación un bloque de texto plano 
        extraído de un CV (el contenido completo del documento) debes devolver **una respuesta estructurada en JSON, cuando tomes el rol de consultor regresa solo una cadena de texto** con tres secciones,
        despues de esto el usuario podria hacerte mas preguntas acerca del CV **La siguiente estructura solo la regresaras 
        una sola vez, porque la siguiente accion del usuario sera hacerte preguntas sobre el CV procesado**:

        1. **Datos extraídos**  
           - **Perfil Personal**  
             • edad  
             • paísResidencia  
             • entidadFederativa  
             • municipioAlcaldía  
           - **Perfil Profesional**  
             • ultimoGradoEstudios  
             • estatusEstudios  
             • tituloPuesto  
             • ultimosTresEmpleos: [ { empresa, puesto, funciones, añosTrabajados, sueldo } ]  
             • areaExperiencia  
             • añosExperiencia  
             • idiomas  
             • nivelIdioma  
           - **Conocimientos**  
             • conocimientos (lista abierta)  
             • herramientasSoftware (lista abierta)  
           - **Preferencias Laborales**  
             • disponibilidadInmediata (Sí/No)  
             • modalidadEmpleo (Home Office/Presencial/Híbrido)  
             • expectativaSalarial (rango)

        2. **Respuestas a las 4 preguntas clave**  
           A partir del mismo CV, responde de forma directa y breve a estas preguntas:  
           1) ¿Tiene más de 6 meses sin trabajar en su trayectoria laboral?  
           2) ¿Vive en Monterrey, N.L. o municipios aledaños?  
           3) ¿Ha trabajado en posiciones similares a vendedor comercial? ¿Cuáles?  
           4) ¿Ha trabajado en más de 1 puesto en un año?  

        3. **Resumen ejecutivo del CV**  
           Un párrafo de 2–3 líneas que destaque el perfil general del candidato:  
           formación, experiencia principal y competencias clave. Y que también incluya las 4 preguntas  
           del punto anterior y sus respuestas.

        **Instrucciones adicionales**  
        - El **input** será solo la cadena de texto del CV; no recibirás JSON ni metadatos.  
        - Tu **output** debe ser **únicamente** un **objeto JSON** con tres propiedades:  
          - `datosExtraidos`  
          - `respuestasPreguntas`  
          - `resumenEjecutivo`  
        - **No** incluyas explicaciones, ni texto extra fuera de ese JSON.  
        - Si algún campo no aparece en el CV, usa `null` o `[]` según corresponda.  
        - No implementes funcionalidades de chat ni mantengas contexto: cada petición es **independiente**.

        **Como primera respuesta devuelves la siguiente estructura, despues solo tomaras el rol de consultor, recibiras
        una pregunta y debes responder con la informacion que se te proporciono en el CV**

        

        **Ejemplo de salida JSON tambien debemos regresar el Id de la conversación**:
            {
              datosExtraidos: {
                perfilPersonal: {
                  edad: int,
                  paisResidencia: string,
                  entidadFederativa: string,
                  municipioAlcaldia: string
                },
                perfilProfesional: {
                  ultimoGradoEstudios: string,
                  estatusEstudios: string,
                  tituloPuesto: string,
                  ultimosTresEmpleos: [
                    {
                      empresa: string,
                      puesto: string,
                      funciones: string,
                      añosTrabajados: int,
                      sueldo: string
                    }
                  ],
                  areaExperiencia: string,
                  añosExperiencia: int,
                  idiomas: [ string ],
                  nivelIdioma: [ string ]
                },
                conocimientos: {
                  conocimientos: [ string ],
                  herramientasSoftware: [ string ]
                },
                preferenciasLaborales: {
                  disponibilidadInmediata: bool,
                  modalidadEmpleo: string,
                  expectativaSalarial: string
                }
              },
              respuestasPreguntas: {
                tieneMasDe6MesesSinTrabajar: bool,
                viveEnMonterreyOMunicipiosAledanos: bool,
                haTrabajadoComoVendedorComercial: bool,
                haTrabajadoEnMasDeUnPuestoEnUnAno: bool
              },
              resumenEjecutivo: string,
              conversationId: " + conversationId
                      + "}";

            // Armamos el promt
            var fullPrompt = systemInstruction
                   + "\n\n--- Comienza CV ---\n"
                   + prompt
                   + "\n--- Fin CV ---\n";

            // Crear la lista e incluir system + CV
            var messages = new List<ChatMessage>
    {
        new SystemChatMessage(systemInstruction),
        new UserChatMessage(prompt)
    };

            // Guardar en cache por 30 minutos
            _cache.Set(conversationId, messages, TimeSpan.FromMinutes(30));

            // Llamada al endpoint
            var response = await AIService.SendPrompt(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 4096,
                Temperature = 0.0f,
                TopP = 1.0f
            });

            // Limpiar la respuesta de posibles delimitadores de markdown
            string cleanedResponse = CleanMarkdownResponse(response);

            // Devolver data procesada
            return cleanedResponse;
        }

        private string CleanMarkdownResponse(string response)
        {
            // Si la respuesta comienza con ```json y termina con ```, quitamos estos delimitadores
            if (response.Trim().StartsWith("```json") && response.Trim().EndsWith("```"))
            {
                // Eliminamos el prefijo ```json y el sufijo ```
                return response.Trim()
                    .Substring("```json".Length)
                    .Substring(0, response.Trim().Length - "```json".Length - "```".Length)
                    .Trim();
            }

            // Si solo está envuelto en comillas triples sin especificar json
            if (response.Trim().StartsWith("```") && response.Trim().EndsWith("```"))
            {
                // Eliminamos el prefijo ``` y el sufijo ```
                return response.Trim()
                    .Substring("```".Length)
                    .Substring(0, response.Trim().Length - "```".Length - "```".Length)
                    .Trim();
            }

            // Si no tiene delimitadores, devolvemos la respuesta tal cual
            return response;
        }

        public async Task<string> ContinueConversationAsync(string conversationId, string question)
        {
            if (!_cache.TryGetValue<List<ChatMessage>>(conversationId, out var messages))
                return "No encontré tu conversación. Por favor envía de nuevo el CV.";

            // Si piden procesar nuevo CV
            //if (question.Trim().Equals("procesar nuevo cv", StringComparison.OrdinalIgnoreCase))
            //{
            //    _cache.Remove(conversationId);
            //    return "Claro, por favor vuelve a enviarme el texto completo del nuevo CV.";
            //}

            var systemInstruction = "Ahora toma el rol de consultor (**Debes devolver solo el texto de la respuesta osea una string**)" +
                "devuelve la respuesta a la siguiente pregunta: ";

            // Añadir la pregunta del usuario
            messages.Add(new UserChatMessage(systemInstruction + question));

            // Llamar al modelo
            var response = await AIService.SendPrompt(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 512,
                Temperature = 0.0f,
                TopP = 1.0f
            });

            // Guardar la respuesta para contexto futuro
            messages.Add(new AssistantChatMessage(response));
            _cache.Set(conversationId, messages, TimeSpan.FromMinutes(30));

            return response;
        }
    }
}
