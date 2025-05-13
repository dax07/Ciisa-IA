using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Caching.Memory;
using OpenAI;
using OpenAI.Chat;
using System;

namespace Ciisa_IA.Services
{
    public class CVService
    {
        private readonly IMemoryCache _cache;
        private readonly ChatClient _chatClient;
        private const int MaxTokens = 512;

        public CVService(IMemoryCache cache)
        {
            _cache = cache;
            var azureClient = new AzureOpenAIClient(
                new Uri("https://dramo-mahtk2xt-eastus2.cognitiveservices.azure.com/"),
                new AzureKeyCredential("9QIGwjNNfWn1OfzY4b6o1z9LRx0lGRu1umwenMGYJNmII69JvOY4JQQJ99BEACHYHv6XJ3w3AAAAACOGtdwh")
            );

            _chatClient = azureClient.GetChatClient("gpt-4o-mini");
        }

        public async Task<string> SendPrompt(string prompt)
        {
            // Definimos el mensaje de sistema con **todas** las reglas y estructura
            var systemInstruction = @"
                Eres un asistente especializado en procesar CVs y responder preguntas sobre ellos.

                Puntos importantes para un analista y de donde pueden salir preguntas:
                1. Perfil Personal:
                   - edad
                   - paisResidencia
                   - entidadFederativa
                   - municipioAlcaldia

                2. Perfil Profesional:
                   - ultimoGradoEstudios
                   - estatusEstudios
                   - tituloPuesto
                   - ultimosTresEmpleos: [
                       empresa, puesto, funciones, añosTrabajados, sueldo
                     ]
                   - areaExperiencia
                   - añosExperiencia
                   - idiomas
                   - nivelIdioma

                3. Conocimientos:
                   - conocimientos
                   - herramientasSoftware

                4. Preferencias Laborales:
                   - disponibilidadInmediata
                   - modalidadEmpleo
                   - expectativaSalarial

                Flujo:
                1) Cuando el usuario envía un texto extenso (su CV), lo procesas y RESPONDES:
                   'CV recibido y procesado con éxito. ¿Qué quieres saber ahora sobre este candidato? 
                    Por ejemplo: ""¿Cuál es el correo electrónico?"" o ""¿Cuál es su país de residencia?""'

                2) A partir de ese momento, cada nuevo input del usuario será una PREGUNTA sobre el CV cargado:
                   a) Primero, normaliza la pregunta corrigiendo pequeños errores de ortografía, tilde, plural/singular 
                        (Viven por Vive, Tienen por Tiene y se intuye que habla del candidato), etc.
                   b) Después de la corrección, proporcionas la respuesta directa extraída del CV.

                3) Las preguntas no necesariamente incluye la palabra 'candidato' o 'el candidato', igualmente intentas responder con la información disponible, teniendo
                    en cuenta que se refiere al candidato.
                   - Si dicha información no existe en el CV, respondes:
                     'No dispongo de esa información en el CV proporcionado o vuelva a formular la pregunta.'

                4) NUNCA salgas de este dominio. Si la pregunta no se relaciona con el CV, responde:
                   'Lo siento, solo puedo responder preguntas sobre el CV proporcionado.'

                No generes JSON en esta fase, salvo si fuera estrictamente necesario para clarificar un dato extraído (pero en este flujo no es necesario).";


            // Crear nuevo ID para conversacion
            string conversationId = Guid.NewGuid().ToString("N");

            // Construimos la conversación: primero el sistema, luego el usuario
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemInstruction),
                new UserChatMessage(prompt)
            };

            // 3) Guardar en cache por 30 minutos
            _cache.Set(conversationId, messages, TimeSpan.FromMinutes(30));

            // 4) Devolver saludo inicial
            return "CV recibido y procesado con éxito. ¿Qué quieres saber ahora? " +
                   "Por ejemplo: “¿Cuál es el correo electrónico?” " + conversationId;
        }

        public async Task<string> ContinuePrompt(string prompt, string conversationId)
        {
            if (!_cache.TryGetValue<List<ChatMessage>>(conversationId, out var messages))
                return "No encontré tu conversación. Por favor envía de nuevo el CV.";

            // Si piden procesar nuevo CV
            /*if (prompt.Trim().Equals("procesar nuevo cv", StringComparison.OrdinalIgnoreCase))
            {
                _cache.Remove(conversationId);
                return "Claro, por favor vuelve a enviarme el texto completo del nuevo CV.";
            }*/

            // Añadir la pregunta del usuario
            messages.Add(new UserChatMessage(prompt));

            // Opciones de la llamada
            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = MaxTokens,
                Temperature = 0.0f,   // temperatura baja para respuestas muy deterministas
                TopP = 1.0f
            };

            // Llamada al endpoint
            var response = await _chatClient.CompleteChatAsync(messages, options);

            // Respuesta de OpenAI
            var answer = response.Value.Content[0].Text.Trim();

            // Guardar la respuesta para contexto futuro
            messages.Add(new AssistantChatMessage(answer));
            _cache.Set(conversationId, messages, TimeSpan.FromMinutes(30));

            return answer;
        }
    }
}
