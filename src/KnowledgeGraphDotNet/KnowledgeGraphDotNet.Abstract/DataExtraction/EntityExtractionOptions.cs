namespace KnowledgeGraphDotNet.Abstract.DataExtraction;

public class EntityExtractionOptions
{
    public int? MaxEntities { get; set; }
    public IEnumerable<Entity>? PreExistingEntities { get; set; }
}