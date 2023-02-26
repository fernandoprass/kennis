using Builder.Domain.Mappers;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Builder.Domain
{
   public interface IBuildLoop
   {
      string Languages(IEnumerable<ProjectLanguage> languages, string defaultLanguage, string layoutBase);
   }
   public class BuildLoop : IBuildLoop
   {
      private readonly ILogger<Build> _logger;

      public BuildLoop(ILogger<Build> logger)
      {
         _logger = logger;
      }

      public string Languages(IEnumerable<ProjectLanguage> languages, string defaultLanguage, string layoutBase)
      {
         var loopItems = languages.ToLoop(defaultLanguage);

         string layout = ParseLoop(layoutBase, loopItems);

         return layout;
      }

      private static string ParseLoop(string layoutBase, IEnumerable<Loop> loopItems)
      {
         var result = new StringBuilder();

         foreach (var item in loopItems)
         {
            string layout = layoutBase;
            layout = layout.Replace(Const.Tag.Loop.Icon, item.Icon);
            layout = layout.Replace(Const.Tag.Loop.Link, item.Link);
            layout = layout.Replace(Const.Tag.Loop.Title, item.Title);
            layout = layout.Replace(Const.Tag.Loop.Description, item.Description);

            result.Append(layout);
         }

         return result.ToString();
      }
   }
}
