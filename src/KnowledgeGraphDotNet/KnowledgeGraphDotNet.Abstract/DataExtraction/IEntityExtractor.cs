namespace KnowledgeGraphDotNet.Abstract.DataExtraction;

public interface IEntityExtractor
{
    Task<IEnumerable<Entity>> ExtractEntitiesAsync(string data,
        EntityExtractionOptions entityExtractionOptions,
        CancellationToken cancellationToken);
}