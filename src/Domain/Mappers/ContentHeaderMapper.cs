using Kennis.Domain.Models;

namespace Kennis.Domain.Mappers
{
   public static class ContentHeaderMapper
   {
      public static Content ToContent(this ContentHeader contentHeader)
      {
         return new Content
         {
            Icon = contentHeader.Icon,
            Title = contentHeader.Title,
            Description = contentHeader.Description,
            Reference = contentHeader.Reference,
            Menu = contentHeader.Menu,
            Categories = contentHeader.Categories,
            Tags = contentHeader.Tags,
            Created = contentHeader.Created,
            Updated = contentHeader.Updated,
            Draft = contentHeader.Draft,
         };
      }
   }
}
