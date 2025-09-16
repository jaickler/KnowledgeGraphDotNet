using System.Runtime.CompilerServices;
using System.Text;
using KnowledgeGraphDotNet.Abstract.DataExtraction;
using KnowledgeGraphDotNet.Abstract.KnowledgeGraph;
using KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;
using KnowledgeGraphDotNet.Core.Neo4j.Config;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace KnowledgeGraphDotNet.Core.Neo4j;

public class Neo4jGraphWriter(
    IDriver driver,
    IChatClient chatClient,
    IOptions<Neo4JGraphOptions> options,
    ILogger<Neo4jGraphWriter>? logger,
    IEntityExtractor entityExtractor) : IGraphWriter
{
    private readonly IChatClient _chatClient = chatClient;
    private readonly IDriver _driver = driver;
    private readonly IEntityExtractor _entityExtractor = entityExtractor;
    private readonly ILogger<Neo4jGraphWriter>? _logger = logger;
    private readonly IOptions<Neo4JGraphOptions> _options = options;

    public IEnumerable<GraphWriteOperation> WriteInformation(string information) =>
        WriteInformationAsync(information,
                CancellationToken.None)
            .ToBlockingEnumerable();

    public async IAsyncEnumerable<GraphWriteOperation> WriteInformationAsync(string information,
        [EnumeratorCancellation] CancellationToken token)
    {
        var cypherWriteQuery = await GetCypherWriteQueryAsync(information, token).ConfigureAwait(false);

        GraphWriteOperation currentOperation;

        try
        {
            await using (var session = _driver.AsyncSession())
            {
                _logger?.LogTrace("Executing write query.");
                var cursor = await session.ExecuteWriteAsync(runner => runner.RunAsync(cypherWriteQuery)).ConfigureAwait(false);

                currentOperation =  new GraphWriteOperation(OperationResult.Succeeded)
                {
                };
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to write information with cypher query.");
            currentOperation =  new GraphWriteOperation(OperationResult.Failed, ex);
        }

        _logger?.LogTrace("Cypher write query completed successfully.");

        yield return currentOperation;
    }


    private async Task<string> GetCypherWriteQueryAsync(string information, CancellationToken token)
    {
        // TODO: Add Entity Extraction.
        var graphSchema = await GetSchema().ConfigureAwait(false);

        var entityExtractionOptions = new EntityExtractionOptions();
        
        var entities = await _entityExtractor.ExtractEntitiesAsync(information,
                entityExtractionOptions,
                token)
            .ConfigureAwait(false);

        var systemMessage = new StringBuilder(_options.Value.ExtractionInstructions)
            .AppendLine("Database Schema (If Available):")
            .AppendLine(graphSchema)
            .AppendLine("Entities to use: ")
            .AppendJoin("\n", entities.Select(entity => entity.ToString()));

        List<ChatMessage> messages =
        [
            new(ChatRole.System, systemMessage.ToString()),
            new(ChatRole.User, information)
        ];

        _logger?.LogTrace("Sending prompt to write cypher query.");
        var chatResult = await _chatClient.GetResponseAsync(messages, cancellationToken: token);

        return chatResult.Text;
    }

    /// <summary>
    ///     Gets the schema of the database.
    /// </summary>
    /// <returns>The schema of the database or null if unable to obtain.</returns>
    private async Task<string?> GetSchema()
    {
        await using var session = _driver.AsyncSession();

        _logger?.LogTrace("Retrieving database schema.");
        try
        {
            var result = await session.RunAsync("CALL db.schema.visualization()");
            var record = await result.SingleAsync();

            var nodes = record["nodes"].As<List<INode>>();
            var relationships = record["relationships"].As<List<IRelationship>>();

            var nodeLabels = nodes.ToDictionary(n => n.ElementId,
                n => n.Labels.FirstOrDefault() ?? "Unknown");

            var sb = new StringBuilder();
            foreach (var rel in relationships)
            {
                var startLabel = nodeLabels[rel.StartNodeElementId];
                var endLabel = nodeLabels[rel.EndNodeElementId];
                sb.AppendLine($"(:{startLabel})-[:{rel.Type}]->(:{endLabel})");
            }

            foreach (var node in nodes)
            {
                var properties = node.Properties.Keys;
                sb.AppendLine($"Node {node.Labels.FirstOrDefault() ?? "Unknown"} Properties:");
                foreach (var prop in properties)
                    sb.AppendLine(prop);
            }

            _logger?.LogTrace("Schema successfully retrieved.");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to retrieve database schema.");
            return null;
        }
    }
}