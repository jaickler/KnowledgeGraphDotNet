namespace KnowledgeGraphDotNet.Abstract.DataExtraction;

public interface IRelationshipExtractor
{
    Task<Relationship> ExtractRelationshipAsync(string data,
        RelationshipExtractionOptions relationshipExtractionOptions,
        CancellationToken cancellationToken);
}

public class RelationshipExtractionOptions(
    IEnumerable<Relationship> preExistingRelationships,
    int? maxRelationships)
{
    public int? MaxRelationships { get; set; } = maxRelationships;
    public IEnumerable<Relationship> PreExistingRelationships { get; set; } = preExistingRelationships;
}