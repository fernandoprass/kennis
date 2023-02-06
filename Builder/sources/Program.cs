using Builder.Domain;
using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Builder
{
    public class KennisBuilder
   {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";

         var serviceProvider = IoC.Configure(projectName);

         var build = serviceProvider.GetService<IBuild>();

         var s = new Site();

         s.Load();

         build.Builder(projectName);
      }

   }
}