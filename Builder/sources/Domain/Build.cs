﻿using Builder.Domain.Builder;
using Builder.Domain.Internationalization;
using Builder.Domain.Layouts;
using Builder.Domain.Models;
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

               var folders = project.Sites.First(s => s.Language == language.Code).Folders;

               var contentList = _site.GetContentList(project.Folders, language.Code, folders.Pages, folders.BlogPosts);

               var loopLanguages = _loop.Languages(project.Languages, project.DefaultLanguage, layoutBase.Loops.Languages);

               var layout = _translate.To(language.Code, project.Folders.Template, layoutBase.Index);

               _logger.LogInformation("Ending create site in {0}", language.Label);
            }
         }
      }

   }
}
