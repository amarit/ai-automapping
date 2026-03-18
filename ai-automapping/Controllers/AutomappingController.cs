using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace ai_automapping.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutomappingController : ControllerBase
    {
        private readonly string _apiKey;

        public AutomappingController(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        }

        /// <summary>
        /// Mapping with AI.
        /// AI gissar mapping baserat på språklig likhet.
        /// </summary>
        /// <param name="asJson"></param>
        /// <returns></returns>
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

        [HttpGet(Name = "GetAutoMappingsUsingEmbeddings")]
        public IActionResult GetAutoMappingsUsingEmbeddings(bool asJson = true)
        {
            string apiKey = _apiKey;

            var client = new EmbeddingClient(
                model: "text-embedding-3-small",
                apiKey: apiKey
            );

                        var sourceFields = new[]
                        {
                "product_title",
                "color_name",
                "fabric_type",
                "product_weight"
            };

                        var destinationFields = new[]
                        {
                "name",
                "color",
                "material",
                "weight"
            };

            var sourceEmbeddings = new Dictionary<string, float[]>();
            var destinationEmbeddings = new Dictionary<string, float[]>();

            foreach (var field in sourceFields)
            {
                OpenAIEmbedding emb = client.GenerateEmbedding(field);
                sourceEmbeddings[field] = emb.ToFloats().ToArray();
            }

            foreach (var field in destinationFields)
            {
                OpenAIEmbedding emb = client.GenerateEmbedding(field);
                destinationEmbeddings[field] = emb.ToFloats().ToArray();
            }

            var mappings = new List<(string source, string destination, double score)>();
            foreach (var source in sourceFields)
            {
                string bestMatch = "";
                double bestScore = -1;

                foreach (var dest in destinationFields)
                {
                    var score = CosineSimilarity(
                        sourceEmbeddings[source],
                        destinationEmbeddings[dest]);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMatch = dest;
                    }
                }

                mappings.Add((source, bestMatch, bestScore));
                Console.WriteLine($"{source} -> {bestMatch} ({bestScore:F3})");
            }

            static double CosineSimilarity(float[] v1, float[] v2)
            {
                double dot = 0;
                double mag1 = 0;
                double mag2 = 0;

                for (int i = 0; i < v1.Length; i++)
                {
                    dot += v1[i] * v2[i];
                    mag1 += v1[i] * v1[i];
                    mag2 += v2[i] * v2[i];
                }

                return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
            }

            return Ok(mappings.Select(m => new { source = m.source, destination = m.destination, score = m.score }));
        }

        private void EmbeddingsBatchExampleRequest()
        {
            var client = new EmbeddingClient(
                model: "text-embedding-3-small",
                apiKey: _apiKey
            );

            var sourceFields = new[]
            {
                "product_title",
                "color_name",
                "fabric_type",
                "product_weight"
            };

            var destinationFields = new[]
            {
                "name",
                "color",
                "material",
                "weight"
            };

            var allTexts = sourceFields.Concat(destinationFields).ToList();

            OpenAIEmbeddingCollection embeddings = client.GenerateEmbeddings(allTexts);

            var vectors = embeddings
                .Select(e => e.ToFloats().ToArray())
                .ToList();

            // Map the embeddings back to their respective fields
            var sourceEmbeddings = new Dictionary<string, float[]>();
            var destinationEmbeddings = new Dictionary<string, float[]>();

            for (int i = 0; i < sourceFields.Length; i++)
            {
                sourceEmbeddings[sourceFields[i]] = vectors[i];
            }

            for (int i = 0; i < destinationFields.Length; i++)
            {
                destinationEmbeddings[destinationFields[i]] = vectors[i + sourceFields.Length];
            }
        }
    }
}
