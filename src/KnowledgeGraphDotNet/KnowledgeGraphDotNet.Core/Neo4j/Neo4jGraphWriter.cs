using System.Text;
using KnowledgeGraphDotNet.Abstract.KnowledgeGraph;
using KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;
using KnowledgeGraphDotNet.Core.Neo4j.Config;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace KnowledgeGraphDotNet.Core.Neo4j;

public class Neo4jGraphWriter(
    IDriver driver,
    IChatClient chatClient,
    IOptions<Neo4JGraphReaderOptions> options,
    ILogger? logger) : IGraphWriter
{
    private readonly IChatClient _chatClient = chatClient;
    private readonly IDriver _driver = driver;
    private readonly IOptions<Neo4JGraphReaderOptions> _options = options;
    private readonly ILogger? _logger = logger;

    public GraphWriteOperation WriteInformation(string information)
    {
        return WriteInformationAsync(information, CancellationToken.None).Result;
    }

    public async Task<GraphWriteOperation> WriteInformationAsync(string information, CancellationToken token)
    {
        var cypherWriteQuery = await GetCypherWriteQueryAsync(information, token);

        try
        {
            await using (var session = _driver.AsyncSession())
            {
                _logger?.LogTrace("Executing write query.");
                await session.ExecuteWriteAsync(runner => runner.RunAsync(cypherWriteQuery));
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to write information with cypher query.");
            return new GraphWriteOperation(OperationResult.Failed, ex);
        }

        _logger?.LogTrace("Cypher write query completed successfully.");

        return new GraphWriteOperation(OperationResult.Succeeded);
    }


    private async Task<string> GetCypherWriteQueryAsync(string information, CancellationToken token)
    {
        var graphSchema = await GetSchema();

        var systemMessage = new StringBuilder(_options.Value.ExtractionInstructions)
            .AppendLine("Database Schema (If Available):")
            .AppendLine(graphSchema);

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