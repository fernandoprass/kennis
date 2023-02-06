using Builder.Domain.Internationalization;
using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Builder.Domain
{
   public interface IBuild
   {
      void Builder(string projectName);
   }

   public class Build : IBuild
   {
      private readonly ILoad _load;
      private readonly ILogger<Build> _logger;
      private readonly ITranslate _translate;
      
      private Project _project;
      private ILayoutBase _layoutBase;

      public Build(ILoad load, ILogger<Build> logger, ITranslate translate)
      {
         _load = load;
         _logger = logger;
         _translate = translate;
      }

      public void Builder(string projectName) {
         _project = _load.Project(projectName);
         _layoutBase = _load.LayoutBase(_project.Folders.Template);


         foreach (var language in _project.Languages)
         {
            _logger.LogWarning(language.Code);
            var layout = _translate.To(language.Code, _project.Folders.Template, _layoutBase.Index);
         }      
      }

   }
}
