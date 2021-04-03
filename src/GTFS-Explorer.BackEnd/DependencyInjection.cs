using GTFS_Explorer.BackEnd.Readers;
using GTFS_Explorer.BackEnd.Repositiories;
using GTFS_Explorer.BackEnd.Schedules;
using GTFS_Explorer.BackEnd.Utilities;
using GTFS_Explorer.Core.Interfaces;
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
            /*
             * Singleton since there will be only 1 GTFS file reader instance
             * throughout the whole application. Otherwise our app would be slow.
             */
            services.AddSingleton(provider => new GTFSFeedReader(env));

            //Repositories
            services.AddScoped<IRoutesRepository, RoutesRepository>();
            services.AddScoped<IStopsRepository, StopsRepository>();
            services.AddScoped<ITimepointRepository, TimepointRepository>();
            services.AddScoped<ICalendarTesterRepository, CalendarTesterRepository>();

            //Other Services
            services.AddSingleton<IEventRegistry, EventRegistry>();
            services.AddScoped<IScheduleBuilder, ScheduleBuilder>();
            services.AddSignalR();

            return services;
        }
    }
}