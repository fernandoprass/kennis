using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces
{
   public interface ITranslationService
   {
      Template Translate(Template template, string language);
   }
}
