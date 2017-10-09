using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace AutoReleaser
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += LoadFromSameFolder;
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            string folderPath = @"D:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin";
            string name = new AssemblyName(args.Name).Name;

            string assemblyPath = Path.Combine(folderPath, name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                if (name == "System.IO") return typeof(File).Assembly;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name + ".dll");
                if(File.Exists(path)) return Assembly.Load(path);

                return null;
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
    }
}