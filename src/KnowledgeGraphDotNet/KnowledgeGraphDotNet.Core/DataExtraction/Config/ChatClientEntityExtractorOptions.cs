using System.ComponentModel.DataAnnotations;

namespace KnowledgeGraphDotNet.Core.DataExtraction.Config;

public record ChatClientEntityExtractorOptions
{
    [Required]
    public required string ExtractionPrompt { get; set; }
}