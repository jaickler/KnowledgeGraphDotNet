using KnowledgeGraphDotNet.Abstract.KnowledgeGraph;
using KnowledgeGraphDotNet.Core.Neo4j.Extensions;
using KnowledgeGraphDotNet.Examples.CreateFromShortStory.Config;
using KnowledgeGraphDotNet.Examples.CreateFromShortStory.Config.Validation;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using OpenAI;

namespace KnowledgeGraphDotNet.Examples.CreateFromShortStory;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

        builder.Logging.AddSimpleConsole(logBuilder
            =>
        {
            logBuilder.ColorBehavior = LoggerColorBehavior.Enabled;
            logBuilder.TimestampFormat = "HH:mm:ss ";
            logBuilder.IncludeScopes = true;
        }).AddFilter(((_) => true));

        builder.Services.AddSingleton<IValidateOptions<OpenAiOptions>, ValidateOpenAiOptions>()
            .AddOptions<OpenAiOptions>()
            .Bind(builder.Configuration.GetSection(nameof(OpenAiOptions)))
            .ValidateOnStart();

        builder.Services.AddSingleton<IChatClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OpenAiOptions>>();
                var client = new OpenAIClient(options.Value.ApiKey);
                return client.GetChatClient(options.Value.Model).AsIChatClient();
            })
            .AddNeo4J(builder.Configuration);

        var app = builder.Build();

        var writer = app.Services.GetRequiredService<IGraphWriter>();

        await foreach(var operation in writer.WriteInformationAsync("Bob is a fool that loves Alice. Alice is a cryptographer that wants to speak with Bob securely to hear what he wnats to tell her.", CancellationToken.None))
        {
            Console.WriteLine(operation.ToString());
        }
    }
}