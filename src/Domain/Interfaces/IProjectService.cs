using Kennis.Domain.Models;
using Myce.Response;

namespace Kennis.Domain.Interfaces;

public interface IProjectService
{
   Project? Load(string projectName);
   Result Validate(Project project);
   void Save();
}
