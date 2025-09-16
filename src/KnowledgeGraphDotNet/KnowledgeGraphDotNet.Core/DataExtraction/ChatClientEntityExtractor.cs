using System.Text;
using System.Text.Json;
using KnowledgeGraphDotNet.Abstract.DataExtraction;
using KnowledgeGraphDotNet.Core.DataExtraction.Config;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KnowledgeGraphDotNet.Core.DataExtraction;

public class ChatClientEntityExtractor(
    ILogger<ChatClientEntityExtractor> logger,
    IChatClient chatClient,
    [FromKeyedServices("ExtractionJsonSerializationOptions")] JsonSerializerOptions jsonSerializationOptions,
    IOptions<ChatClientEntityExtractorOptions> options) : IEntityExtractor
{
    private readonly ILogger<ChatClientEntityExtractor> _logger = logger;
    private readonly IChatClient _chatClient = chatClient;
    private readonly JsonSerializerOptions _jsonSerializationOptions = jsonSerializationOptions;
    private readonly IOptions<ChatClientEntityExtractorOptions> _options = options;

    public async Task<IEnumerable<Entity>> ExtractEntitiesAsync(string data,
        EntityExtractionOptions entityExtractionOptions,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var systemMessage = new StringBuilder(_options.Value.ExtractionPrompt);
        
        if (entityExtractionOptions?.PreExistingEntities is not null)
            systemMessage.AppendLine("Pre-existing entities:")
                .AppendLine(string.Join(", ",
                    entityExtractionOptions.PreExistingEntities
                        .Select(e => e.ToString())));

        List<ChatMessage> messages =
        [
            new(ChatRole.System, systemMessage.ToString()),
            new(ChatRole.User, data)
        ];

        _logger?.LogTrace("Sending prompt to extract entities from data.");
        
        var chatResult = await _chatClient.GetResponseAsync<IEnumerable<Entity>>(messages,
            serializerOptions: _jsonSerializationOptions,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var entities = chatResult.Result;
        
        return entities;
    }
}