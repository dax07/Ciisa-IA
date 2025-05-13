using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Ciisa_IA.Services
{
    public class AIService
    {
        private readonly ChatClient _chatClient;

        public AIService()
        {
            var azureClient = new AzureOpenAIClient(
                new Uri("https://dramo-mahtk2xt-eastus2.cognitiveservices.azure.com/"),
                new AzureKeyCredential("9QIGwjNNfWn1OfzY4b6o1z9LRx0lGRu1umwenMGYJNmII69JvOY4JQQJ99BEACHYHv6XJ3w3AAAAACOGtdwh")
            );

            _chatClient = azureClient.GetChatClient("gpt-4o-mini");
        }

        public async Task<string> SendPrompt(string prompt)
        {
            var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

            var requestOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 4096,
                Temperature = 1.0f,
                TopP = 1.0f
            };

            var response = await _chatClient.CompleteChatAsync(messages, requestOptions);

            return response.Value.Content[0].Text;
        }
    }
}
