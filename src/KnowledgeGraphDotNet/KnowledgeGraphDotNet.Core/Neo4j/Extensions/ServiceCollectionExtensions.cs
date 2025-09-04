using KnowledgeGraphDotNet.Abstract.KnowledgeGraph;
using KnowledgeGraphDotNet.Core.Neo4j.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace KnowledgeGraphDotNet.Core.Neo4j.Extensions;

/// <summary>
///     Contains extension methods to streamline adding Graph interactions with DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the basic servcies and configuration to interact with Knowledge Graphs.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add services to.</param>
    /// <param name="configuration">The configuration to use for setup.</param>
    /// <returns>This service collection to chain calls.</returns>
    public static IServiceCollection AddNeo4J(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddOptions<Neo4JDriverOptions>()
            .Bind(configuration.GetSection(nameof(Neo4JDriverOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        serviceCollection.AddOptions<Neo4JGraphOptions>()
            .Bind(configuration.GetSection(nameof(Neo4JGraphOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        serviceCollection.AddScoped<IDriver>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<Neo4JDriverOptions>>().Value;
                return GraphDatabase.Driver(options.Uri, AuthTokens.Basic(options.Username,
                    options.Password));
            })
            .AddScoped<IGraphWriter, Neo4jGraphWriter>();

        return serviceCollection;
    }

    /// <summary>
    ///     Adds the basic services to interact with the knowledge graph.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration to use for setup.</param>
    /// <param name="driverFactory">The factory method to create the instance of <see cref="IDriver" />.</param>
    /// <returns>This service collection to use </returns>
    public static IServiceCollection AddNeo4J(this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Func<(IServiceProvider, IConfiguration), IDriver> driverFactory)
    {
        serviceCollection.AddOptionsWithValidateOnStart<Neo4JGraphOptions>("Neo4j");
        serviceCollection.AddScoped<IDriver>(sp => driverFactory.Invoke((sp, configuration)))
            .AddScoped<Neo4jGraphWriter>();

        return serviceCollection;
    }

    public static IServiceCollection AddNeo4J(this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Func<IServiceProvider, IDriver> driverFactory)
    {
        serviceCollection.AddOptionsWithValidateOnStart<Neo4JGraphOptions>("Neo4j");
        serviceCollection.AddScoped<IDriver>(driverFactory)
            .AddScoped<Neo4jGraphWriter>();

        return serviceCollection;
    }
}