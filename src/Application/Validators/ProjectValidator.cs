using Kennis.Domain.Models;
using Myce.Response;
using Myce.FluentValidator;

namespace Kennis.Domain.Validators;

public class ProjectValidator : FluentValidator<ProjectValidator>
{
   private readonly Project _project;
   public ProjectValidator(Project project)
   {
      _project = project;
   }
   public Result Validate()
   {
      throw new NotImplementedException();
   }
}
