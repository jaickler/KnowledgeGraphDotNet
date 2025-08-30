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
    }
}