using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;

namespace ProxyServerTester.Presentation.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        app.MapOpenApi(pattern: "/openapi/{documentName}/openapi.yaml");
        
        app.UseSwagger(options =>
        {
            options.RouteTemplate = "/openapi/{documentName}/openapi.yaml";
        });
    
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1/openapi.yaml", "Proxy Server Tester API");
        
            options.RoutePrefix   = "swagger";
            options.DocumentTitle = "ProxyServerTester";

            options.EnableFilter();
            options.DisplayOperationId();
            options.DisplayRequestDuration();
        });

        app.MapScalarApiReference(options =>
        {
            options.AddDocument("v1", "Proxy Server Tester API", "/openapi/v1/openapi.yaml");
        
            options.Title               = "Proxy Server Tester API";
            options.Layout              = ScalarLayout.Modern;
            options.ForceThemeMode      = ThemeMode.Dark;
            options.Theme               = ScalarTheme.Default;
            options.HideSearch          = false;
            options.Agent               = new ScalarAgentOptions { Disabled = true };
            options.DefaultOpenAllTags  = true;
            options.EnabledClients      = [ScalarClient.HttpClient, ScalarClient.Axios];
            options.TagSorter           = TagSorter.Alpha;
            options.OperationSorter     = OperationSorter.Alpha;
            options.OpenApiRoutePattern = "scalar";
            options.BundleUrl           = "https://cdn.jsdelivr.net/npm/@scalar/api-reference";
            options.ShowOperationId     = true;
            options.ShowDeveloperTools  = DeveloperToolsVisibility.Localhost;
        });

        app.UseReDoc(options =>
        {
            options.DocumentTitle  = "Proxy Server Tester API";
            options.RoutePrefix    = "re-doc";
            options.SpecUrl        = "/openapi/v1/openapi.yaml";
        });
        
        return app;
    }
}