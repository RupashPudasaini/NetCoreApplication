using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;
using OpenAI.Chat;
using OpenAI;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace OpenAIWebApp.Pages
{
    public class OpenAIModel : PageModel
    {
        private readonly string _apiKey;
        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        public string Summary { get; set; }
        public OpenAIClient client { get; set; }

        public OpenAIModel()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            client = new(_apiKey);
        }

        [BindProperty]
        public string UserInput { get; set; }
        public string ResponseText { get; set; }

        public IActionResult OnPostChat()
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


            try
            {
                ChatClient chatClient = client.GetChatClient(requestBody.model);

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
        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a file.");
                return Page();
            }

            using var reader = new StreamReader(UploadedFile.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();

            Summary = GetSummary(fileContent);

            return Page();
        }

        private string GetSummary(string fileContent)
        {

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                prompt = "Summarize the following file:\n\n" + fileContent,
                max_tokens = 2000
            };

            ChatClient chatClient = client.GetChatClient(requestBody.model);

            ChatCompletion chatCompletion = chatClient.CompleteChat([
                new UserChatMessage(requestBody.prompt)
            ]);
            //check to see if there are any responses
            if (chatCompletion.Content.Count > 0)
            {
                ResponseText = chatCompletion.Content[0].Text.ToString();
            }

            return chatCompletion.Content[0].Text;
        }

    }
}

