using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Tauron.Application.Common.Updater.Provider;
using Tauron.Application.Common.Updater.Service;
using Tauron.Application.Implement;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class UpdateManager : IUpdateManager
    {
        private const string StepCommand = "Step";
        private const string Step2Parm = "Setup";
        private const string Step3Parm = "Completion";
        private const string KillProcessCommand = "KillProcess";
        private const string TargetCommand = "Target";
        private const string BasePathCommand = "BasePath";
        private const string VersionCommand = "NewVersion";

        private class PrivateUpdate : IUpdate
        {
            public PrivateUpdate(Release release)
            {
                Release = release;
            }

            public Release Release { get; }
        }

        public InstallerStade Setup()
        {
            HashSet<CommandLineProcessor.Command> commands = new HashSet<CommandLineProcessor.Command>(CommandLineProcessor.ParseCommandLine(DebuggerService.GetCommandLine(), DebuggerService.SkipFirst));

            var stepcommand = commands.FirstOrDefault(c => c != null && c.Name == StepCommand);
            if (stepcommand == null) return InstallerStade.NoUpdate;

            switch (stepcommand.Parms[0])
            {
                case Step2Parm:
                    KillProcess(commands.First(c => c.Name == KillProcessCommand).Parms[0]);
                    string basePath = GetCommandValue(commands, BasePathCommand);
                    string target = GetCommandValue(commands, TargetCommand);
                    Version newVersion = Version.Parse(GetCommandValue(commands, VersionCommand));

                    ExecuteSetup(basePath, target, newVersion);
                    return InstallerStade.Shudown;
                case Step3Parm:
                    KillProcess(commands.First(c => c.Name == KillProcessCommand).Parms[0]);
                    UpdaterService.Configuration.Provider.UpdaterFilesLocation.DeleteDirectory(true);
                    UpdaterService.Configuration.StartCleanUp?.Invoke();
                    return InstallerStade.Compled;
                default:
                    return InstallerStade.NoUpdate;
            }
        }

        public IUpdate CheckUpdate()
        {
            Version currentVersion = UpdaterService.Configuration.CurrentVersion;
            var currrent = GetCurrent();
            Version newesVersion = currrent.Version;

            if (newesVersion > currentVersion)
                return new PrivateUpdate(currrent);
            return null;
        }

        public void InstallUpdate(IUpdate update)
        {
            var configuration = UpdaterService.Configuration;
            var downloaded = UpdaterService.InstallManager.DownloadUpdate(update);

            string files = configuration.Provider.Preperator.Prepare(downloaded);

            string targetFile = files.CombinePath(configuration.SetupFile);
            var processId = Process.GetCurrentProcess().Id;

            string commandLine = $"-{KillProcessCommand} {processId} -{StepCommand} {Step2Parm} -{TargetCommand} \"{downloaded}\" -{BasePathCommand} \"{AppDomain.CurrentDomain.BaseDirectory}\" -{VersionCommand} {configuration.CurrentVersion}";

            if (DebuggerService.Debug)
                DebuggerService.Result = commandLine;
            else
                Process.Start(targetFile, commandLine);
        }

        public IUpdate GetLasted()
        {
            return new PrivateUpdate(GetCurrent());
        }

        private Release GetCurrent()
        {
            var temp = UpdaterService.Configuration.Provider.GetReleases().ToArray();
            return temp.OrderByDescending(t => t.Version).First();
        }

        private void KillProcess(string id)
        {
            if (DebuggerService.Debug)
                return;

            try
            {
                var process = Process.GetProcessById(Int32.Parse(id));

                if (!process.WaitForExit(TimeSpan.FromSeconds(30).Milliseconds))
                    process.Kill();
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidOperationException || e is Win32Exception)
            {
            }

        }

        private string GetCommandValue(HashSet<CommandLineProcessor.Command> commands, string name)
        {
            return commands.First(c => c.Name == name).Parms[0];
        }

        private void ExecuteSetup(string basePath, string filePath, Version oldVersion)
        {
            UpdaterService.InstallManager.ExecuteSetup(basePath, filePath, oldVersion);

            string targetFile = basePath.CombinePath(UpdaterService.Configuration.StartFile);
            var processId = Process.GetCurrentProcess().Id;

            string commandLine = $"-{KillProcessCommand} {processId} -{StepCommand} {Step3Parm}";

            if (DebuggerService.Debug)
                DebuggerService.Result = commandLine;
            else
                Process.Start(targetFile, commandLine);
        }
    }
}