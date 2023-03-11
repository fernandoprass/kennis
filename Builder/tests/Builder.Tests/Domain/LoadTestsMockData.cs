namespace Builder.Tests.Domain
{
   internal static class LoadTestsMockData
   {
      public static string ContentListJsonFile()
      {
         return @"[
            {
               ""Type"": 0,
               ""Filename"": ""2023-01-09-first-page.md"",
               ""Keywords"": ""kennis,demo"",
               ""Published"": null,
               ""Slug"": null,
               ""Url"": ""en/pages/hello-world.html"",
               ""Delete"": false,
               ""Icon"": null,
               ""Title"": ""Hello World"",
               ""Description"": ""My first page"",
               ""Reference"": ""hello-world"",
               ""Menu"": true,
               ""Categories"": [ ""kennis"", ""demo"" ],
               ""Tags"": [ ""kennis"", ""demo"" ],
               ""Created"": ""2023-01-02T14:05:00+00:00"",
               ""Updated"": null,
               ""Draft"": false
            },
            {
               ""Type"": 1,
               ""Filename"": ""2023-01-09-hello-world.md"",
               ""Keywords"": ""kennis"",
               ""Published"": null,
               ""Slug"": null,
               ""Url"": ""en/blog/posts/kennis/hello-world.html"",
               ""Delete"": false,
               ""Icon"": null,
               ""Title"": ""Hello World"",
               ""Description"": ""My first post"",
               ""Reference"": ""hello-world"",
               ""Menu"": false,
               ""Categories"": [ ""kennis"" ],
               ""Tags"": null,
               ""Created"": ""2023-01-02T14:05:00+00:00"",
               ""Updated"": null,
               ""Draft"": false
            },
            {
               ""Type"": 1,
               ""Filename"": ""2023-01-21-second-post.md"",
               ""Keywords"": ""kennis,post"",
               ""Published"": null,
               ""Slug"": null,
               ""Url"": ""en/blog/posts/kennis/second-post.html"",
               ""Delete"": false,
               ""Icon"": null,
               ""Title"": ""Second Post"",
               ""Description"": ""My second post"",
               ""Reference"": ""second-post"",
               ""Menu"": false,
               ""Categories"": [ ""kennis"" ],
               ""Tags"": [ ""kennis"", ""post"" ],
               ""Created"": ""2023-01-02T15:05:00"",
               ""Updated"": null,
               ""Draft"": false
            }
          ]";
      }

      public static string ContentHeaderYamlFile()
      {
         return @"
            title: content title
            description: content description 
            created: 2023-01-02 15:05:00
            updated:
            categories: [kennis]
            tags : [kennis, demo]
            reference: hello-world
            menu: true
            draft: false
           ";
      }

      public static string MdFileWithYamlHeader()
      {
         return "---\nfield: value\n---\n# MD Command";
      }

      public static string TemplateJsonFile()
      {
         return @"{
				""Languages"": [ ""en"", ""pt-br"" ],
				""Index"": ""index.html"",
				""Page"": null,
				""Blog"": null,
				""BlogArchive"": null,
				""BlogCategories"": null,
				""BlogPost"": null,
				""BlogTags"": null,
				""Loops"": {
					""BlogArchive"": ""loop.blog.archive.html"",
					""BlogCategories"": ""loop.blog.categories.html"",
					""BlogPostLast10"": ""loop.blog.posts.last10.html"",
					""BlogPostLast5"": null,
					""BlogPostLast3"": null,
					""BlogPosts"": null,
					""BlogTags"": ""loop.blog.tags.html"",
					""Languages"": ""loop.languages.html"",
					""Menu"": ""loop.menu.html"",
					""SocialMedia"": ""loop.socialMedia.html""
				}
			}";
      }
   }
}
