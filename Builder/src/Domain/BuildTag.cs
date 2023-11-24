using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IBuildTag
   {
      string Content(string layoutBase, Content content, string dateTimeFormat);
      string Index(string layoutBase, ProjectSite site);
   }

   public class BuildTag : IBuildTag
   {
      private readonly ILogger<BuilderService> _logger;
      private Dictionary<string, string> _siteTags { get; set; }
      private Dictionary<string, string> _contentTags { get; set; }

      public BuildTag(ILogger<BuilderService> logger)
      {
         _logger = logger;
      }

      public string Index(string layoutBase, ProjectSite site)
      {
         var tags = GetSiteTags(site);

         return ParseTags(layoutBase, tags);
      }

      public string Content(string layoutBase, Content content, string dateTimeFormat)
      {
         var tags = GetContentTags(content, dateTimeFormat);

         return ParseTags(layoutBase, tags);
      }

      private static string ParseTags(string layoutBase, Dictionary<string, string> tags)
      {
         if (layoutBase.IsNull())
         {
            return string.Empty;
         }

         string layout = layoutBase;

         foreach (var tag in tags)
         {
            layout = layout.Replace(tag.Key, tag.Value.EmptyIfIsNull());
         }

         return layout;
      }

      private Dictionary<string, string> GetSiteTags(ProjectSite site)
      {
         if (_siteTags.IsNotNull())
         {
            return _siteTags;
         }

         _siteTags = new Dictionary<string, string>
         {
            { Const.Tag.Site.Title, site.Title },
            { Const.Tag.Site.Subtitle, site.Subtitle },
            { Const.Tag.Site.Description, site.Description },
            { Const.Tag.Site.Keywords, site.Keywords },
            { Const.Tag.Site.Language, site.Language.Code },
            { Const.Tag.Site.Modified, site.Modified.ToString(site.DateTimeFormat) },
            { Const.Tag.Site.GoogleAnalyticTrackingId, site.GoogleAnalyticTrackingId },
           
            //todo missing tags    
            // { Const.Tag.Site.Index, site.Index },
            //_tags.Add(Const.Tag.Site.BaseUrl, site.);

            { Const.Tag.Site.Author.Name, site.Author.Name },
            { Const.Tag.Site.Author.Avatar, site.Author.Avatar },
            { Const.Tag.Site.Author.Bio, site.Author.Bio },
            { Const.Tag.Site.Author.Location, site.Author.Location },
            { Const.Tag.Site.Author.Email, site.Author.Email }
         };

         return _siteTags;
      }

      private Dictionary<string, string> GetContentTags(Content content, string dateTimeFormat)
      {
         if (_contentTags.IsNotNull())
         {
            return _contentTags;
         }
         //todo check Text, Categories, Tags and Date format
         _contentTags = new Dictionary<string, string>
         {
            { Const.Tag.Content.Title, content.Title },
            { Const.Tag.Content.Description, content.Description },
            { Const.Tag.Content.Slug, content.Slug },
            { Const.Tag.Content.Reference, content.Reference },
            { Const.Tag.Content.Categories, content.Categories.ToString() },
            { Const.Tag.Content.Tags, content.Tags.ToString() },
            { Const.Tag.Content.Created, content.Created.ToString(dateTimeFormat) },
            { Const.Tag.Content.Updated, content.Published?.ToString(dateTimeFormat) },
            //  { Const.Tag.Content.Text, content.tex },
         };

         return _contentTags;
      }
   }
}
