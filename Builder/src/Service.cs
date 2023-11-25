using Builder.Domain;
using Microsoft.Extensions.DependencyInjection;
using Myce.Wrappers;
using Myce.Wrappers.Contracts;
using Serilog;

namespace Builder
{
    public static class Service
   {
      public static ServiceProvider Configure(string projectName)
      {
         var services = new ServiceCollection();

         ConfigureSerilogService(services, projectName);

         ConfigureBusinessServices(services);

         return services.BuildServiceProvider();
      }

      private static void ConfigureSerilogService(IServiceCollection services, string projectName)
      {
         var serilogLogger = new LoggerConfiguration()
          .WriteTo.Console()
          .WriteTo.File($"{projectName}.log")
          .CreateLogger();

         services.AddLogging(builder =>
         {
            builder.AddSerilog(logger: serilogLogger, dispose: true);
         });
      }

      private static ServiceProvider ConfigureBusinessServices(IServiceCollection services)
      {
         return services.AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                        .AddSingleton<IFileWrapper, FileWrapper>()
                        .AddSingleton<IPathWrapper, PathWrapper>()
                        .AddSingleton<ILoadService, LoadService>()
                        .AddSingleton<ISave, Save>()
                        .AddSingleton<ITemplateService, TemplateService>()
                        .AddSingleton<IProjectService, ProjectService>()
                        .AddScoped<IBuilderService, BuilderService>()
                        .AddScoped<IBuildSiteService, BuilderSiteService>()
                        .AddScoped<IBuildLoop, BuildLoop>()
                        .AddScoped<IBuildTag, BuildTag>()
                        .AddScoped<IData, Data>()
                        .AddScoped<ITemplateTranslateService, TemplateTranslateService>()
                        .BuildServiceProvider();
      }
   }
}
