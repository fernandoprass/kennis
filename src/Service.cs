using Kennis.Domain;
using Microsoft.Extensions.DependencyInjection;
using Myce.Wrappers;
using Myce.Wrappers.Contracts;
using Serilog;

namespace Kennis
{
    public static class Service
   {
      public static ServiceProvider Configure(string projectName, string logLevel)
      {
         var services = new ServiceCollection();

         ConfigureSerilogService(services, projectName, logLevel);

         ConfigureBusinessServices(services);

         return services.BuildServiceProvider();
      }

      private static void ConfigureSerilogService(IServiceCollection services, string projectName, string logLevel)
      {
         var logEventLevel = logLevel.Equals("verbose")
            ? Serilog.Events.LogEventLevel.Verbose
            : Serilog.Events.LogEventLevel.Information;

         var log = new LoggerConfiguration()
           .WriteTo.Console(logEventLevel)
            .WriteTo.File($"{projectName}.log")
            .CreateLogger();

         services.AddLogging(builder =>
         {
            builder.AddSerilog(logger: log, dispose: true);
         });
      }

      private static ServiceProvider ConfigureBusinessServices(IServiceCollection services)
      {
         return services.AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                        .AddSingleton<IFileWrapper, FileWrapper>()                        
                        .AddSingleton<ILoadService, LoadService>()
                        .AddSingleton<ILogService, LogService>()
                        .AddSingleton<IPathWrapper, PathWrapper>()
                        .AddSingleton<IProjectService, ProjectService>()
                        .AddSingleton<ISaveService, SaveService>()
                        .AddSingleton<ITemplateService, TemplateService>()
                        .AddSingleton<ITranslationService, TranslationService>()
                        .AddScoped<IBuilderService, BuilderService>()
                        .AddScoped<IBuildSiteService, BuilderSiteService>()
                        .AddScoped<IBuildLoop, BuildLoop>()
                        .AddScoped<IBuildTag, BuildTag>()
                        .AddScoped<IDataService, DataService>()
                        .BuildServiceProvider();
      }
   }
}
