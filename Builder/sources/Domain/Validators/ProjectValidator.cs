using Builder.Domain.Models;
using Myce.Response;
using Myce.Validation;

namespace Builder.Domain.Validators
{
   public class ProjectValidator : EntityValidator<ProjectValidator>
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
}
