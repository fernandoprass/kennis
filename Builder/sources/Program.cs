﻿using Builder.Domain;
using Builder.Domain.Configuration;
using Builder.Domain.Layouts;
using Microsoft.Extensions.DependencyInjection;

namespace Builder
{
   public class KennisBuilder
   {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";

         var serviceProvider = IoC.Configure(projectName);

         var project = Project.Get(projectName);

         var layoutBase = serviceProvider.GetService<ILayoutBase>();
         var build = serviceProvider.GetService<IBuild>();

         layoutBase.Get(project.Template);

         build.Builder(project, layoutBase);
      }

   }
}