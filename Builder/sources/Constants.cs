namespace Kennis.Builder.Constants
{
   public static class LocalEnvironment
   {
      public static class File
      {
         public const string Project = "project.json";
         public const string Template = "template.json";
      }

      public static class Folder
      {
         public const string Templates = @"templates\";
         public const string Projects = @"projects\";
         public const string Sites = @"sites\";
      }
   }

   public static class Tag
   {
      public static class Site
      {
         public const string Title = "{@site.title}";
         public const string Subtitle = "{@site.subtitle}";
         public const string Description = "{@site.description}";
         public const string Keywords = "{@site.keywords}";
         public const string BaseUrl = "{@site.baseUrl}";
         public const string Language = "{@site.language}";
         public const string GoogleAnalyticTrackingId = "{@site.googleAnalyticTrackingId}";
         public const string Modified = "{@site.modified}";
      }

      public static class Author
      {
         public const string Name = "{@author.name}";
         public const string Avatar = "{@author.avatar}";
         public const string Bio = "{@author.bio}";
         public const string Location = "{@author.location}";
         public const string Email = "{@author.email}";
      }

      public static class Loop
      {
         public const string Icon = "{@loop.icon}";
         public const string Description = "{@loop.description}";
         public const string Link = "{@loop.link}";
         public const string Title = "{@loop.title}";
      }

      public static class Content
      {
         public const string Language = "{@content.language}";
         public const string Author = "{@content.author}";
         public const string Description = "{@content.description}";
         public const string Title = "{@content.title}";
         public const string Slug = "{@content.slug}";
         public const string Reference = "{@content.reference}";
         public const string Text = "{@content.content}";
         public const string Categories = "{@content.categories}";
         public const string Tags = "{@content.tags}";
         public const string PublicationDate = "{@content.publicationDate}";
         public const string UpdateDate = "{@content.updateDate}";
      }
   }
}
