namespace KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;

public class GraphReadOperation<T>(
    OperationResult resultStatus,
    T? result,
    Exception? exception = null) : GraphOperation(resultStatus, exception)
{
    /// <summary>
    ///     The result from the read operation if successful.
    /// </summary>
    public T? Result { get; } = result;
}