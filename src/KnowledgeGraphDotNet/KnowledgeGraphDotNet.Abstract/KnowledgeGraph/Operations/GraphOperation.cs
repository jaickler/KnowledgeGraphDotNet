namespace KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;

public abstract class GraphOperation(
    OperationResult resultStatus,
    Exception? exception = null)
{
    public OperationResult ResultStatus { get; init; } = resultStatus;
    public Exception? Exception { get; init; } = exception;
}