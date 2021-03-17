using GTFS_Explorer.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GTFS_Explorer.FrontEnd.Installers
{
    public static class InstallerExtensions 
    {
        /// <summary>
        /// Extension to install services from classes 
        /// that implement the IInstaller interface
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Configuration</param>
        public static void InstallServicesInAssembly(
            this IServiceCollection services, 
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
               typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.InstallServices(services, configuration, hostingEnvironment));
        }
    }
}