﻿using Builder.Domain.Models;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface ITemplateService
   {
      bool Load(string templateFolder);
      bool Translate(string language);

   }
   public class TemplateService : ITemplateService
   {
      private readonly ILoad _load;
      private readonly ITemplateTranslateService _templateTranslateService;

      private Template _template;

      public TemplateService(ILoad load, ITemplateTranslateService templateTranslateService)
      {
         _load = load;
         _templateTranslateService = templateTranslateService;
      }

      public bool Load(string templateFolder)
      {
         _template = _load.Template(templateFolder);

         return _template.IsNotNull();
      }

      public bool Translate(string language)
      { 
         return true;
      }
   }
}
