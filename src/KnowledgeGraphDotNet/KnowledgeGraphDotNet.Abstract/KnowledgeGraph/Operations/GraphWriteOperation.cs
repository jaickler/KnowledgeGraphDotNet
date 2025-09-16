using System.Text;
using KnowledgeGraphDotNet.Abstract.DataExtraction;

namespace KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;

public class GraphWriteOperation : GraphOperation
{
    /// <summary>
    ///     Creates an instance of the <see cref="GraphWriteOperation" /> class.
    /// </summary>
    /// <param name="resultStatus">The status of the operation.</param>
    /// <param name="exception">The exception from the failed operation if one occured.</param>
    public GraphWriteOperation(OperationResult resultStatus,
        Exception? exception = null) : base(resultStatus, exception)
    {
        CreatedEntities = [];
        MatchedEntities = [];
        CreatedRelationships = [];
        MatchedRelationships = [];
    }

    public GraphWriteOperation(OperationResult resultStatus,
        IEnumerable<Entity> createdEntities,
        IEnumerable<Entity> matchedEntities,
        IEnumerable<Relationship> createdRelationships,
        IEnumerable<Relationship> matchedRelationships) : base(resultStatus, null)
    {
        CreatedEntities = createdEntities?? [];
        MatchedEntities = matchedEntities?? [];
        CreatedRelationships = createdRelationships?? [];
        MatchedRelationships = matchedRelationships?? [];
    }

    public IEnumerable<Relationship> MatchedRelationships { get; init; }

    public IEnumerable<Relationship> CreatedRelationships { get; init; }

    public IEnumerable<Entity> MatchedEntities { get; init; }

    public IEnumerable<Entity> CreatedEntities { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Matched Entities:");
        foreach (var matchedEntity in MatchedEntities)
            sb.AppendLine(matchedEntity?.ToString());

        sb.AppendLine()
            .AppendLine("Created Entities:");

        foreach (var createdEntity in CreatedEntities)
            sb.AppendLine(createdEntity?.ToString());
        
        sb.AppendLine()
            .AppendLine("Matched Relationships:");

        foreach (var matchedEntity in MatchedEntities)
            sb.AppendLine(matchedEntity?.ToString());

        sb.AppendLine()
            .AppendLine("Created Relationships:");
       
        foreach (var createdRelationship in CreatedRelationships)
            sb.AppendLine(createdRelationship?.ToString());
        
        return sb.ToString();
    }
}