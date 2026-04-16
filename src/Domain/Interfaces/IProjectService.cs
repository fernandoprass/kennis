using Kennis.Domain.Models;
using Myce.Response;

namespace Kennis.Domain.Interfaces;

public interface IProjectService
{
   Task<Project?> LoadAsync(string projectName);
   Result Validate(Project project);
   Task SaveAsync();
}
