using System.ComponentModel.DataAnnotations;

namespace KnowledgeGraphDotNet.Examples.CreateFromShortStory.Config;

internal class OpenAiOptions
{
    [Required] public required string ApiKey { get; set; }

    [Required] public required string Model { get; set; }
}