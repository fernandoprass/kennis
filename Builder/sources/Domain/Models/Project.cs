﻿namespace Builder.Domain.Models
{
   public class Project
   {
      public string Name { get; set; }
      public string BaseUrl { get; set; }
      public string DefaultLanguage { get; set; }
      public byte Pagination { get; set; }
      public string Template { get; set; }
      public IEnumerable<ProjectLanguage> Languages { get; set; }
      public IEnumerable<ProjectSite> Sites { get; set; }
      public ProjectFolder Folders { get; set; }
   }
}
