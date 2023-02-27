using Builder.Domain.Internationalization;
using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IBuild
   {
      void Builder(string projectName);
   }

   public class Build : IBuild
   {
      private readonly ILoad _load;
      private readonly IBuildLoop _loop;
      private readonly ILogger<Build> _logger;
      private readonly ITranslate _translate;
      private readonly IData _site;

      private Project project;
      private ILayoutBase layoutBase;

      public Build(ILoad load,
         IBuildLoop loop,
         ILogger<Build> logger,
         IData site,
         ITranslate translate)
      {
         _load = load;
         _loop = loop;
         _logger = logger;
         _site = site;
         _translate = translate;
      }

      public void Builder(string projectName)
      {
         project = _load.Project(projectName);

         if (project.IsNotNull())
         {
            //todo add validate here

            layoutBase = _load.LayoutBase(project.Folders.Template);

            foreach (var language in project.Languages)
            {
               _logger.LogInformation("Starting create site in {0}", language.Label);

               var site = project.Sites.First(s => s.Language == language.Code);

               var contentList = _site.GetContentList(project.Folders, language.Code, site.Folders.Pages, site.Folders.BlogPosts);

               string layout = ParseLoops(site, contentList);

               layout = _translate.To(language.Code, project.Folders.Template, layout);

               _logger.LogInformation("Ending create site in {0}", language.Label);
            }
         }
      }

      private string ParseLoops(ProjectSite site, List<Content> contentList)
      {
         var loopLanguages = _loop.Languages(project.Languages, project.DefaultLanguage, layoutBase.Loops.Languages);

         var loopSocialMedia = _loop.SocialMedia(site.Author.SocialMedia, layoutBase.Loops.SocialMedia);

         var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
         var loopMenu = _loop.Menu(menuList, layoutBase.Loops.Menu);

         var layout = layoutBase.Index;

         layout = layout.Replace(Const.Tag.Site.Loop.Languages, loopLanguages);
         layout = layout.Replace(Const.Tag.Site.Loop.Menu, loopMenu);
         layout = layout.Replace(Const.Tag.Site.Loop.SocialMedia, loopSocialMedia);

         return layout;
      }
   }
}
