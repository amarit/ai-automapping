using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace ai_automapping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomappingController : ControllerBase
    {
        private readonly string _apiKey;

        public AutomappingController(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        }

        [HttpGet(Name = "GetAutoMappings")]
        public IActionResult GetAutoMappings(bool asJson = true)
        {
            var client = new ChatClient(model: "gpt-4o-mini", _apiKey);

            var sourceSchema = new[]
            {
    "product_title",
    "color_name",
    "material_info",
    "product_weight"
};

            var destinationSchema = new[]
            {
    "name",
    "color",
    "material",
    "weight"
};

            var prompt = $"""
Map the following source fields to the destination fields.

Source fields:
{string.Join("\n", sourceSchema)}

Destination fields:
{string.Join("\n", destinationSchema)}

Return JSON array with objects containing:
- source
- destination
""";

            if (asJson)
            {
                prompt += "\nReturn ONLY valid JSON.\r\nDo not include markdown, explanation, or text.";
            }

            var response = client.CompleteChat(prompt);

            return Ok(response.Value.Content[0].Text);
        }
    }
}
