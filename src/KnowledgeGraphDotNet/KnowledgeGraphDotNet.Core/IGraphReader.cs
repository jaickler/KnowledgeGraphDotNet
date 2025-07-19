namespace KnowledgeGraphDotNet.Core;

public interface IGraphReader
{
    /// <summary>
    /// Gets the answer from the knowledge graph in a conversational string.
    /// </summary>
    /// <param name="query">The conversational question to answer.</param>
    /// <returns>The answer from the knowledge graph in a conversational format.</returns>
    string GetResult(string query);
}