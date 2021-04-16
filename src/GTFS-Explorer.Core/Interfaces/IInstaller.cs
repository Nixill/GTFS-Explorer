using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GTFS_Explorer.Core.Interfaces
{
    public interface IInstaller
    {
        void InstallServices(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment);
    }
}