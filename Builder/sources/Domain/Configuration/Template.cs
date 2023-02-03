using Builder.Domain.Wrappers;

namespace Builder.Domain.Configuration
{
   public class Template
   {
      private readonly IFileWrapper _file;

      public IEnumerable<string> Languages { get; set; }
      public IEnumerable<TemplateHtmlFile> Index { get; set; }
      public IEnumerable<TemplateHtmlFile> Blog { get; set; }
      public IEnumerable<TemplateHtmlFile> Page { get; set; }
      public IEnumerable<TemplateHtmlFile> Post { get; set; }
      public TemplateLoopHtmlFile Loops { get; set; }
   }
}