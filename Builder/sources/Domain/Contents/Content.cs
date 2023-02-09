namespace Builder.Domain.Contents
{
    public class Content : ContentHeader
    {
      public string Filename { get; set; }
      public DateTime? Published { get; set; }
   }
}
