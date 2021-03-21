using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace GTFS_Explorer.BackEnd.Extensions
{
    public static class ElectronExtensions
    {
        /// <summary>
        /// Extension to combine the GTFS file storage directory with the given GTFS file name
        /// </summary>
        /// <param name="env">Host Environment</param>
        /// <param name="fileName">Name of the GTFS file</param>
        /// <returns>Path of GTFS file storage directory with the given file name</returns>
        public static string GetGTFSFilePath(this App app, IWebHostEnvironment env, string fileName)
        {
            return Path.Combine(Electron.App.CreateGTFSFileDir(env), fileName);
        }

        /// <summary>
        /// Extension to create the directory of where the GTFS files are copied to
        /// </summary>
        /// <param name="env">Host Environment</param>
        /// <returns>Created directory of GTFS files storage</returns>
        public static string CreateGTFSFileDir(this App app, IWebHostEnvironment env)
        {
            /*If running electron: yourPath\GTFS-Explorer.FrontEnd\obj\Host\bin\wwwroot\tempFiles*/
            /*If running browser: yourPath\GTFS-Explorer.FrontEnd\wwwroot\tempFiles*/
            string dir = $"{env.WebRootPath}\\tempFiles\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        /// <summary>
        /// Extension to get GTFS file directory
        /// </summary>
        /// <param name="env">Host Environment</param>
        /// <returns>Returns the directory if it exists, otherwise returns null</returns>
        public static string GetGTFSFileDir(this App app, IWebHostEnvironment env)
        {
            string dir = $"{env.WebRootPath}\\tempFiles\\";
            if (!Directory.Exists(dir))
                return null;
            return dir;
        }

        /// <summary>
        /// Extension to delete the GTFS storage directory
        /// </summary>
        /// <param name="env">Host Environment</param>
        public static void DeleteGTFSFileDir(this App app, IWebHostEnvironment env)
        {
            var dir = Electron.App.GetGTFSFileDir(env);
            if(dir != null)
                DeleteDirectory(dir);
        }

        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}