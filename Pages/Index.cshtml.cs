using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoryCreator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Question { get; set; } = String.Empty;
        public string ResponseContent { get; set; } = String.Empty;

        public void OnPost()
        {
            // Configure OpenAI client
            string openAIEndpoint = "";
            string openAIKey = "";
            string openAIDeploymentName = "";

            OpenAIClient client = new(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));

            // Configure search service
            string searchEndpoint = "";
            string searchKey = "";
            string searchIndex = "";

            AzureCognitiveSearchChatExtensionConfiguration codewolfConfig = new()
            {
                SearchEndpoint = new Uri(searchEndpoint),
                Authentication = new OnYourDataApiKeyAuthenticationOptions(searchKey),
                IndexName = searchIndex
            };

            // Set up the AI chat query/completion
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages = { new ChatRequestUserMessage(Question) },
                AzureExtensionsOptions = new AzureChatExtensionsOptions
                {
                    Extensions = { codewolfConfig }
                },
                DeploymentName = openAIDeploymentName
            };

            // Send request to Azure OpenAI model
            ChatCompletions chatCompletionsResponse = client.GetChatCompletions(chatCompletionsOptions);

            ResponseContent = chatCompletionsResponse.Choices[0].Message.Content;
        }
    }
}