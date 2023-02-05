using Builder.Domain.Configuration;
using Builder.Domain.Internationalization;
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
      private readonly ITranslate _translate;

      public Build(ILogger<Build> logger, ITranslate translate)
      {
         _logger = logger;
         _translate = translate;
      }

      public void Builder(Project project, ILayoutBase layoutBase) { 
         foreach(var language in project.Languages)
         {
            _logger.LogWarning(language.Code);
            _translate.To(language.Code, layoutBase);
         }      
      }

   }
}
