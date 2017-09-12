using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Octokit;
using Tauron.Application.Common.Updater;
using Tauron.Application.Common.Updater.Impl;
using Tauron.Application.Common.Updater.Service;

namespace ConsoleApp1
{
    class Program
    {
        static int Main(string[] args)
        {
            var temp = AssemblyDefinition.ReadAssembly(Assembly.GetEntryAssembly().Location);
            var test = temp.Name.Version;
            //var client = new GitHubClient(new ProductHeaderValue("Cello-Manager-Updater", "0.1.0"));
            //var temp = client.Repository.Release.GetLatest("Tauron1990", "CelloManager").Result;

            //WebClient nclient = new WebClient();
            
            //foreach (var asset in temp.Assets)
            //{
            //    nclient.DownloadFile(asset.BrowserDownloadUrl, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setup.zip"));    
            //}

            //DebuggerService.StartDebug();
            //UpdaterService.SetGithubProvider("Tauron1990", "CelloManager", new ProductHeaderValue("Cello-Manager-Updater", "0.0.0"), s => Version.Parse(s.TrimStart('v')));

            //UpdaterService.Configuration.CurrentVersion = new Version();
            //UpdaterService.Configuration.FileSelector = release => release.Files.First();
            //UpdaterService.Configuration.SetupFile = "CelloManager.exe";
            //UpdaterService.Configuration.StartFile = "CelloManager.exe";

            //var update = UpdaterService.UpdateManager.CheckUpdate();
            //UpdaterService.UpdateManager.InstallUpdate(update);

            //InstallerStade stade = InstallerStade.NoUpdate;
            //stade = UpdaterService.UpdateManager.Setup();
            //stade = UpdaterService.UpdateManager.Setup();
            //stade = UpdaterService.UpdateManager.Setup();

            //if(stade == InstallerStade.NoUpdate) return 5;

            return 0;
        }
    }
}
