using GTFS_Explorer.BackEnd;
using GTFS_Explorer.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GTFS_Explorer.FrontEnd.Installers
{
    public class BackendInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, 
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            services.AddBackend(environment);
        }
    }
}