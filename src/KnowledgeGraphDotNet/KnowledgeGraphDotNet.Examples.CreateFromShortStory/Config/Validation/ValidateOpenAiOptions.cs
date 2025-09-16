using Microsoft.Extensions.Options;

namespace KnowledgeGraphDotNet.Examples.CreateFromShortStory.Config.Validation;

[OptionsValidator]
public partial class ValidateOpenAiOptions : IValidateOptions<OpenAiOptions>
{
    
}