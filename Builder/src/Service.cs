using Builder.Domain;
using Builder.Domain.Internationalization;
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
         return services.AddSingleton<IFileWrapper, FileWrapper>()
                        .AddSingleton<IPathWrapper, PathWrapper>()
                        .AddSingleton<IDirectoryWrapper, DirectoryWrapper>()
                        .AddSingleton<ILoad, Load>()
                        .AddSingleton<ISave, Save>()
                        .AddSingleton<ILayoutService, LayoutService>()
                        .AddSingleton<IProjectService, ProjectService>()
                        .AddScoped<IBuildService, BuildService>()
                        .AddScoped<IBuildSite, BuildSite>()
                        .AddScoped<IBuildLoop, BuildLoop>()
                        .AddScoped<IBuildTag, BuildTag>()
                        .AddScoped<IData, Data>()
                        .AddScoped<ITranslate, Translate>()
                        .BuildServiceProvider();
      }
   }
}
