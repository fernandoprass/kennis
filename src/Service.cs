using Kennis.Application.Services;
using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Kennis.Infrastructure.Logging;
using Kennis.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Myce.Wrappers;
using Myce.Wrappers.Contracts;
using Serilog;

namespace Kennis
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
         var log = new LoggerConfiguration()
           .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
            .WriteTo.File($"{projectName}{Const.Extension.Log}")
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
