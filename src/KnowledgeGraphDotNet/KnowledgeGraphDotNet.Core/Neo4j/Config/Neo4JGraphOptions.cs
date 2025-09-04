namespace KnowledgeGraphDotNet.Core.Neo4j.Config;

public record Neo4JGraphOptions
{
    public required string ExtractionInstructions { get; set; }
}