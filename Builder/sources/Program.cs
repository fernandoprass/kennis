using Builder.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Builder
{
   public class KennisBuilder
   {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";

         var serviceProvider = Service.Configure(projectName);

         var build = serviceProvider.GetService<IBuild>();

         build.Builder(projectName);
      }

   }
}