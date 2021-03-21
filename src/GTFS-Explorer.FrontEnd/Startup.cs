using ElectronNET.API;
using ElectronNET.API.Entities;
using GTFS_Explorer.BackEnd.SignalR;
using GTFS_Explorer.Core.Interfaces;
using GTFS_Explorer.FrontEnd.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GTFS_Explorer.FrontEnd
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        private IEventRegistry EventRegistry { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * Now this method takes care of installing services for us 
             * everytime we create an Installer class!
             */
            services.InstallServicesInAssembly(Configuration, HostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            IEventRegistry eventRegistry)
        {
            EventRegistry = eventRegistry;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<EventsHub>("/eventsHub");
            });

            if (HybridSupport.IsElectronActive)
            {
                CreateWindow();
            }
        }

        private async void CreateWindow()
        {
            const int MIN_HEIGHT = 800;
            const int MIN_WIDTH = 1150;

            var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Height = MIN_HEIGHT,
                Width = MIN_WIDTH
            });

            window.SetMinimumSize(MIN_WIDTH, MIN_HEIGHT);
            window.OnClosed += () => {
                Electron.App.Quit();
            };

            EventRegistry.RegisterEvents();
        }
    }
}