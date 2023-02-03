using Builder.Domain.Configuration;
using Builder.Domain.Layouts;
using Microsoft.Extensions.Logging;

namespace Builder.Domain
{
   public interface IBuild
   {
      void Builder(Project project, ILayoutBase layoutBase);
   }
   public class Build : IBuild
   {
      private readonly ILogger<Build> _logger;

      public Build(ILogger<Build> logger)
      {
         _logger = logger;
      }

      public void Builder(Project project, ILayoutBase layoutBase) { 
         foreach(var language in project.Languages)
         {
            _logger.LogWarning(language.Code);
         }      
      }
   }
}
