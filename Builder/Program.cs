using Builder.Domain.Configuration;
using Kennis.Builder.Domain;

public class KennisBuilder
{
   static void Main(string[] args)
   {
      var projectName = "KennisDemo";

      var project = Project.Get(projectName);

      var layout = new Layout();

      layout.Get(project.Template);
   }

}