namespace Kennis.Domain.Interfaces;

public interface IBuilderService
{
   Task BuildAsync(string projectName, bool rebuildAll);
}
