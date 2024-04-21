using mychatgpt.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace mychatgpt.APIServices
{
    public class GPTAPIService
    {
        private readonly ILogger<GPTAPIService> _logger; 

        private static PostData postData = new PostData
        {
            model = "gpt-3.5-turbo",
            messages = new List<Message>(),
            temperature = 0.7F
        };

        private static int totalTokens = 0;

        public GPTAPIService(ILogger<GPTAPIService> logger)
        {
            _logger = logger;
        }

        public async Task<string> postSend(string inputMessage)
        {
            postData.messages.Add(new Message
            {
                role = "user",
                content = inputMessage
            });

            var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.openai.com/v1/chat/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "api-key");

            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("completions", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var postResponse = JsonSerializer.Deserialize<ResponseData>(responseContent);

                await showLoggerInfo(postResponse, inputMessage);

                postData.messages.Add(new Message
                {
                    role = "assistant",
                    content = postResponse.choices[0].message.content
                });

                messagesLimit();

                return postResponse.choices[0].message.content;
            }
            else
            {
                _logger.LogInformation($"error: {response.StatusCode}");
            }

            return null;
        }

        private void messagesLimit()
        {
            if (postData.messages.Count >= 10)
            {
                postData.messages.RemoveAt(0);
                postData.messages.RemoveAt(0);
            }
        }
        private async Task showLoggerInfo(ResponseData postResponse, string inputMessage)
        {
            _logger.LogInformation
                    ($"" +
                        $"\nrequest: {inputMessage}\n" +
                        $"response: {postResponse.choices[0].message.content}" +

                        $"\ntokents spended: request: {postResponse.usage.prompt_tokens} tokens " +
                        $"\nresponse: {postResponse.usage.completion_tokens} tokens " +
                        $"\nresponse total: {totalTokens += postResponse.usage.total_tokens} tokens" +

                        $"\n\ntotal: {totalTokens} tokens"
                    );
        }
    }
}
