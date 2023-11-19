﻿using Builder.Domain;
using Builder.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Myce.Extensions;
using System;

namespace Builder
{
   public class KennisBuilder
   {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";
         bool regenerateAllSite = true;

         var serviceProvider = Service.Configure(projectName);

         var project = LoadProject(serviceProvider, projectName);

         var build = serviceProvider.GetService<IBuild>();

         build.Builder(project, regenerateAllSite);
      }


      private static Project LoadProject(ServiceProvider serviceProvider, string projectName)
      {
         var projectService = serviceProvider.GetService<IProjectService>();
         var project = projectService.Load(projectName);

         //todo add validate here
         if (project.IsNotNull())
         {
            projectService.ProjectSiteUpdateLanguageData(project.DefaultLanguageCode, project.Sites);
         }

         return project;
      }
   }
}