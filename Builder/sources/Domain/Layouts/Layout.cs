namespace Builder.Domain.Layouts
{
   public interface ILayout
   {
      LayoutTemplate Index { get; set; }
      LayoutTemplate Blog { get; set; }
      LayoutTemplate Page { get; set; }
      LayoutTemplate Post { get; set; }
      LayoutLoop Loops { get; set; }
   }

   public class Layout : ILayout
   {
      public LayoutTemplate Index { get; set; }
      public LayoutTemplate Blog { get; set; }
      public LayoutTemplate Page { get; set; }
      public LayoutTemplate Post { get; set; }
      public LayoutLoop Loops { get; set; }
   }
}