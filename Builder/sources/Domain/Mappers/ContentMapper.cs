using Builder.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Domain.Mappers
{
   public static class ContentMapper
   {
      public static Content Merge(this Content content, ContentHeader contentHeader)
      {
         content.Icon = contentHeader.Icon;
         content.Title = contentHeader.Title;
         content.Description = contentHeader.Description;
         content.Reference = contentHeader.Reference;
         content.Menu = contentHeader.Menu;
         content.Categories = contentHeader.Categories;
         content.Tags = contentHeader.Tags;
         content.Created = contentHeader.Created;
         content.Updated = contentHeader.Updated;
         content.Draft = contentHeader.Draft;

         return content;
      }
   }
}
