using System.Diagnostics.CodeAnalysis;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpaceBattle.Server;
using Swashbuckle.AspNetCore.Swagger;

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "Test API";
            o.Version = "1";
            o.Description = "API Description";
        });

        services.AddSingleton(new SwaggerOptions());
    }

    public static void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<SwaggerMiddleware>();
        app.UseSwaggerUI();

        app.UseServiceModel(builder =>
        {
            builder.AddService<SpaceServer>();
            builder.AddServiceWebEndpoint<SpaceServer, ISpaceServer>(new WebHttpBinding
            {
                MaxReceivedMessageSize = 5242880,
                MaxBufferSize = 65536,
            }, "spacebattle", behavior =>
            {
                behavior.HelpEnabled = true;
                behavior.AutomaticFormatSelectionEnabled = true;
            });
        });
    }
}
