using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IBuildTag
{
   string Content(string templateBase, Content content, string dateTimeFormat);
   string Index(string templateBase, ProjectSite site);
}
