using System.ComponentModel.DataAnnotations;

namespace KnowledgeGraphDotNet.Core.Neo4j.Config;

public record Neo4JDriverOptions
{
    [Required] public required string Uri { get; set; }

    [Required] public required string Username { get; set; }

    [Required] public required string Password { get; set; }
}