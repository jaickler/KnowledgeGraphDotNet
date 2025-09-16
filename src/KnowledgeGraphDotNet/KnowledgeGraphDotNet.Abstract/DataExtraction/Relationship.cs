namespace KnowledgeGraphDotNet.Abstract.DataExtraction;

public class Relationship
{
    public string Name { get; set; }
    public IDictionary<string, string>  Attributes { get; set; }
}