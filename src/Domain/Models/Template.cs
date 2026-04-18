namespace Kennis.Domain.Models;


public class Template
{
   public IEnumerable<string> Languages { get; set; }
   public string DefaultLanguage { get; set; }
   public string TemplateEngine { get; set; }
   public required IEnumerable<string> Assets { get; set; }
   public required TemplatePages Pages { get; set; }
}