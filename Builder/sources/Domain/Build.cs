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
      private readonly ISite _site;


      private Project project;
      private ILayoutBase layoutBase;

      public Build(ILoad load, 
         ILogger<Build> logger, 
         ISite site,
         ITranslate translate)
      {
         _load = load;
         _logger = logger;
         _site = site;
         _translate = translate;
      }

      public void Builder(string projectName) {
         project = _load.Project(projectName);
         layoutBase = _load.LayoutBase(project.Folders.Template);


;        foreach (var language in project.Languages)
         {
            var folders = project.Sites.FirstOrDefault(s => s.Language == language.Code)?.Folders;
            _site.Load(project.Folders, language.Code, folders.Pages, folders.BlogPosts);

            _logger.LogWarning(language.Code);
            var layout = _translate.To(language.Code, project.Folders.Template, layoutBase.Index);
         }      
      }

   }
}
