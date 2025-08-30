using KnowledgeGraphDotNet.Abstract.KnowledgeGraph.Operations;

namespace KnowledgeGraphDotNet.Abstract.KnowledgeGraph;

public interface IGraphReader
{
    /// <summary>
    ///     Gets the answer from the knowledge graph.
    /// </summary>
    /// <param name="query">The conversational question to answer.</param>
    /// <returns>The answer from the knowledge graph.</returns>
    GraphReadOperation<T> GetResult<T>(string query);

    /// <summary>
    ///     Gets the answer from the knowledge graph.
    /// </summary>
    /// <param name="query">The conversational question to answer.</param>
    /// <param name="token">The <see cref="CancellationToken" /> to use for cancelling this operation.</param>
    /// <returns>The answer from the knowledge graph.</returns>
    Task<GraphReadOperation<T>> GetResultAsync<T>(string query, CancellationToken token);
}