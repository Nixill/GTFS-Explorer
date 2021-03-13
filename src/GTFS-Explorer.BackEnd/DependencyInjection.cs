using ElectronNET.API;
using GTFS_Explorer.BackEnd.Extensions;
using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.Core.Interfaces.RepoInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GTFS_Explorer.BackEnd
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds services from backend to ServiceCollection
        /// </summary>
        /// <returns>Service Collection from backend</returns>
        public static IServiceCollection AddBackend(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSingleton(provider => new GTFSFeedReader(env));
            services.AddScoped<IRoutesRepository, RoutesRepository>();
            return services;
        }
    }
}