using System.IO;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using GTFS_Explorer.BackEnd.Extensions;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            });

            if (HybridSupport.IsElectronActive)
            {
                CreateWindow(env);
            }
        }

        private async void CreateWindow(IWebHostEnvironment env)
        {
            const int MIN_HEIGHT = 780;
            const int MIN_WIDTH = 1100;

            var options = new BrowserWindowOptions();
            options.Height = MIN_HEIGHT;
            options.Width = MIN_WIDTH;

            var window = await Electron.WindowManager.CreateWindowAsync(options);
            window.SetMinimumSize(MIN_WIDTH, MIN_HEIGHT);
            window.OnClosed += () => {
                Electron.App.Quit();
            };

            Electron.App.BeforeQuit += (x) =>
            {
                Electron.App.DeleteStoredGTFSFiles(env);
                return Task.CompletedTask;
            };
        }
    }
}