using Kennis.Domain.Models;
using Myce.FluentValidator;
using Myce.Response;

namespace Kennis.Application.Validators
{
   public static class AppSettingsValidatorExtensions
   {
      public static Result Validate(this AppSettings appSettings)
      {
         var validator = new FluentValidator<AppSettings>();

         validator
            .RuleFor(a => a.Language).IsRequired()
            .RuleFor(a => a.ProjectName).IsRequired()
            .RuleFor(a => a.Folders.Projects).IsRequired()
            .RuleFor(a => a.Folders.Templates).IsRequired();

         var result = validator.Validate(appSettings);
         
         return result ? Result.Success() : Result.Failure(validator.Messages);
      }
   }
}
