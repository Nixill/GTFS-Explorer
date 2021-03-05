using GTFS_Explorer.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GTFS_Explorer.FrontEnd.Installers
{
    public class RazorPagesInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorPages();
        }
    }
}