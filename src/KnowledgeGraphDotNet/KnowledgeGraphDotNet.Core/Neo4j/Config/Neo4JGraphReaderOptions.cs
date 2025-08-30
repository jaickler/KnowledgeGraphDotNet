namespace KnowledgeGraphDotNet.Core.Neo4j.Config;

public record Neo4JGraphReaderOptions
{
    public required string ExtractionInstructions { get; init; }
}