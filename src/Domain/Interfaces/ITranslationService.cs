using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces
{
   public interface ITranslationService
   {
      Task<Template> TranslateAsync(Template template, string language);
   }
}
