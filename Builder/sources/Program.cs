using Builder.Domain.Configuration;
using Builder.Domain.Layouts;
using Builder.Domain.Wrappers;
using Microsoft.Extensions.DependencyInjection;

public class KennisBuilder
{
   static void Main(string[] args)
   {
      var serviceProvider = ConfiguraDependecyInjection();

      var projectName = "KennisDemo";

      var project = Project.Get(projectName);

      var layoutBase = serviceProvider.GetService<ILayoutBase>();

      layoutBase.Get(project.Template);
   }

   private static ServiceProvider ConfiguraDependecyInjection()
   {
      //setup our DI
      return new ServiceCollection()
          .AddSingleton<IFileWrapper, FileWrapper>()
          .AddScoped<ILayoutBase, LayoutBase>()
          .BuildServiceProvider();
   }
}