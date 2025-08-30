using KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;

namespace KnowledgeGraphDotNet.Abstract.KnowledgeGraph;

public interface IGraphWriter
{
    /// <summary>
    ///     Writes the specified information to the graph after parsing.
    /// </summary>
    /// <param name="information">The information to parse and write to the graph.</param>
    /// <returns>The status of the operation.</returns>
    GraphWriteOperation WriteInformation(string information);

    /// <summary>
    ///     Writes the specified information to the graph after parsing.
    /// </summary>
    /// <param name="information">The information to parse and write to the graph.</param>
    /// <param name="token">The <see cref="CancellationToken" /> to use to cancel the operation.</param>
    /// <returns>A task representing the write operation.</returns>
    Task<GraphWriteOperation> WriteInformationAsync(string information, CancellationToken token);
}