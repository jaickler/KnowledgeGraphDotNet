using System.Text.Json.Serialization;
using KnowledgeGraphDotNet.Abstract.DataExtraction;

namespace KnowledgeGraphDotNet.Core.DataExtraction.Config;

[JsonSerializable(typeof(IEnumerable<Entity>))]
[JsonSerializable(typeof(Entity))]
public partial class Neo4JExtractionJsonContext : JsonSerializerContext;