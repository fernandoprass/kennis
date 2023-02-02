using Builder.Domain.Configuration;
using Builder.Domain.Wrappers;
using Kennis.Builder.Domain;
using Microsoft.Extensions.DependencyInjection;

public class KennisBuilder
{
   static void Main(string[] args)
   {
      var serviceProvider = ConfiguraDependecyInjection();

      var projectName = "KennisDemo";

      var project = Project.Get(projectName);

      var layout = serviceProvider.GetService<ILayout>();

      layout.Get(project.Template);
   }

   private static ServiceProvider ConfiguraDependecyInjection()
   {
      //setup our DI
      return new ServiceCollection()
          .AddSingleton<IFileWrapper, FileWrapper>()
          .AddScoped<ILayout, Layout>()
          .BuildServiceProvider();
   }
}