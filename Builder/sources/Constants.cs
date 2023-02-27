namespace Kennis.Builder.Constants
{
   public static class Const
   {
      public static class Extension
      {
         public const string I18n = ".json";
         public const string Content = ".md";
         public const string WebPages = ".html";
      }

      public static class File
      {
         public const string Project = "project.json";
         public const string ContentList = "content.json";
         public const string Template = "template.json";
      }

      public static class Folder
      {
         public const string Templates = @"templates\";
         public const string TemplatesTranslations = @"i18n\";
         public const string Projects = @"projects\";
         public const string Sites = @"sites\";
         public const string Pages = @"pages\";
         public const string Posts = @"posts\";
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
            public const string Index = "{@site.index}";
            public static class Loop
            {
               public const string BlogArchive = "{@loop.blog.archive}";
               public const string BlogCategories = "{@loop.blog.categories}";
               public const string BlogPostLast10 = "{@loop.blog.posts.last10}";
               public const string BlogPostLast5 = "{@loop.blog.posts.last5}";
               public const string BlogPostLast3 = "{@loop.blog.posts.last3}";
               public const string BlogPosts = "{@loop.blog.posts}";
               public const string BlogTags = "{@loop.blog.tags}";
               public const string Languages = "{@loop.languages}";
               public const string Menu = "{@loop.menu}";
               public const string SocialMedia = "{@loop.socialMedia}";
            }
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
            public const string Link = "{@loop.link}";
            public const string Title = "{@loop.title}";
            public const string Description = "{@loop.description}";
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
}
