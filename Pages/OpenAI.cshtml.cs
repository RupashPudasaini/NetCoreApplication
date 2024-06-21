using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using OpenAI.Chat;
using OpenAI;


namespace OpenAIWebApp.Pages
{
    public class OpenAIModel : PageModel
    {
        private readonly string _apiKey;


        public OpenAIModel()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        }

        [BindProperty]
        public string UserInput { get; set; }
        public string ResponseText { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserInput))
            {
                return Page();
            }

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                prompt = UserInput,
                max_tokens = 100
            };

            OpenAIClient client = new(_apiKey);

            try
            {
                ChatClient chatClient = client.GetChatClient(requestBody.model);
                //a simple call for the user prompt request
                ChatCompletion chatCompletion = chatClient.CompleteChat([
                    new UserChatMessage(requestBody.prompt)
                ]);
                //check to see if there are any responses
                if (chatCompletion.Content.Count > 0)
                {
                    ResponseText = chatCompletion.Content[0].Text.ToString();
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }
    }
}
