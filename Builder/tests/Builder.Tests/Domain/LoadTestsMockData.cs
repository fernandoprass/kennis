namespace Builder.Tests.Domain
{
	internal static class LoadTestsMockData
   {
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
