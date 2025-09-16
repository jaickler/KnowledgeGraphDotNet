using System.Text;
using System.Text.Json.Serialization;

namespace KnowledgeGraphDotNet.Abstract.DataExtraction;

public record Entity
{
    public string Name { get; set; }
    public IDictionary<string, string> Attributes { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Name: {Name}")
            .AppendLine("Attributes: ")
            .AppendJoin('\n',
                Attributes.Select(attribute
                    => $"Key: {attribute.Key}\tValue: {attribute.Value}"));

        return sb.ToString();
    }
}