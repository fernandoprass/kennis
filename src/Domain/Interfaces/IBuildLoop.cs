using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IBuildLoop
{
   string BlogPostsLastX(IEnumerable<Content> posts, string templateBase, int numberOfPosts);

   string Languages(IEnumerable<Language> languages, string defaultLanguage, string templateBase);

   string Menu(IEnumerable<Content> menu, string templateBase);

   string SocialMedia(IEnumerable<AuthorSocialMedia> socialMedia, string templateBase);
}
