namespace Builder.Domain.Layouts
{
    public class LayoutTemplate
    {
        public string Template { get; set; }
        public List<LayoutTemplatePreprocessed> TemplatesPreprocessed { get; set; }
    }
}
