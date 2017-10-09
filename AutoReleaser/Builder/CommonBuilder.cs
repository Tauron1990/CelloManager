using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using AutoReleaser.Datastore;
using AutoReleaser.SolutionLoader;
using Ionic.Zip;
using Ionic.Zlib;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Mono.Cecil;
using Mvvm;
using NUnit.ConsoleRunner;
using NUnit.Engine;
using Octokit;
using Octokit.Internal;
using SharpCompress.Compressors.LZMA;
using Application = System.Windows.Application;
using CompressionMode = SharpCompress.Compressors.CompressionMode;
using FileMode = System.IO.FileMode;
using ILogger = Microsoft.Build.Framework.ILogger;
using Project = Microsoft.Build.Evaluation.Project;

namespace AutoReleaser.Builder
{
    public class CommonBuilder : BindableBase
    {
        private class PersonalLogger : ILogger
        {
            private readonly Action<string> _newLineWriter;
            private readonly Action<bool> _errorNotify;
            private IEventSource _eventSource;

            public PersonalLogger(Action<string> newLineWriter, Action<bool> errorNotify)
            {
                _newLineWriter = newLineWriter;
                _errorNotify = errorNotify;
            }

            public void Initialize(IEventSource eventSource)
            {
                _eventSource = eventSource;
                _eventSource.BuildFinished += EventSourceOnBuildFinished;
                _eventSource.BuildStarted += EventSourceOnBuildStarted;
                _eventSource.ProjectStarted += EventSourceOnProjectStarted;
                _eventSource.ErrorRaised += EventSourceOnErrorRaised;
            }

            private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
            {
                _newLineWriter("\t" + FormatEventMessage("error", e.Subcategory, e.Message, e.Code, e.File,
                                   e.ProjectFile, e.LineNumber, e.EndLineNumber, e.ColumnNumber, e.EndColumnNumber,
                                   e.ThreadId) + Environment.NewLine);
                _errorNotify(true);
            }

            private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs e)
            {
                _newLineWriter($"\tStart Project: {Path.GetFileName(e.ProjectFile)}... " + Environment.NewLine);
            }
            private void EventSourceOnBuildStarted(object sender, BuildStartedEventArgs e)
            {
                _errorNotify(false);
                _newLineWriter("Start Building..." + Environment.NewLine);
            }
            private void EventSourceOnBuildFinished(object sender, BuildFinishedEventArgs e)
            {
                _newLineWriter("Building Completed" + Environment.NewLine);
            }

            public void Shutdown()
            {
                _eventSource.BuildFinished -= EventSourceOnBuildFinished;
                _eventSource.BuildStarted -= EventSourceOnBuildStarted;
                _eventSource.ProjectStarted -= EventSourceOnProjectStarted;
                _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
                _eventSource = null;
            }

            public LoggerVerbosity Verbosity { get; set; }
            public string Parameters { get; set; }

            private static string FormatEventMessage(string category, string subcategory, string message, string code, string file, string projectFile, int lineNumber, int endLineNumber, int columnNumber, int endColumnNumber, int threadId)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(file))
                    stringBuilder.Append("MSBUILD : ");
                else
                {
                    stringBuilder.Append("{1}");
                    if (lineNumber == 0)
                        stringBuilder.Append(" : ");
                    else if (columnNumber == 0)
                        stringBuilder.Append(endLineNumber == 0 ? "({2}): " : "({2}-{7}): ");
                    else if (endLineNumber == 0)
                        stringBuilder.Append(endColumnNumber == 0 ? "({2},{3}): " : "({2},{3}-{8}): ");
                    else if (endColumnNumber == 0)
                        stringBuilder.Append("({2}-{7},{3}): ");
                    else
                        stringBuilder.Append("({2},{3},{7},{8}): ");
                }
                if (!string.IsNullOrEmpty(subcategory))
                    stringBuilder.Append("{9} ");
                stringBuilder.Append("{4} ");
                stringBuilder.Append(code == null ? ": " : "{5}: ");
                if (message != null)
                    stringBuilder.Append("{6}");
                if (projectFile != null && !string.Equals(projectFile, file))
                    stringBuilder.Append(" [{10}]");
                if (message == null)
                    message = string.Empty;
                string format = stringBuilder.ToString();
                string[] array = message.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                StringBuilder stringBuilder2 = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder2.Append(string.Format(CultureInfo.CurrentCulture, format, threadId, Path.GetFileName(file), lineNumber, columnNumber, category, code, array[i], endLineNumber, endColumnNumber, subcategory, Path.GetFileName(projectFile)));
                    if (i < array.Length - 1)
                    {
                        stringBuilder2.AppendLine();
                    }
                }
                return stringBuilder2.ToString();
            }
        }
        private class TestPersonalLogger : TextWriter
        {
            private readonly Action<string> _logger;

            public TestPersonalLogger(Action<string> logger)
            {
                _logger = logger;
            }

            public override Encoding Encoding { get; } = Encoding.UTF8;

            public override void Write(char value) => _logger(value.ToString());

            public override void Write(string value) => _logger(value);

            public override void WriteLine(string value) => _logger(value + Environment.NewLine);
        }

        private readonly Paragraph _console;
        private readonly Action _startProcess;
        private readonly Action _stopProcess;
        private readonly Dispatcher _dispatcher;
        private readonly Options _options;

        private ReleaseItem _releaseItem;
        private SolutionBrowser _solutionBrowser;
        private string _tempPath;
        private ProjectCollection _projectCollection;
        private string _lastBuildPath;
        private bool _buildFailed;

        private bool? _testOk;
        private bool? _versionOk;
        private bool? _buildOk;
        private bool? _uploadOk;

        public CommonBuilder(Paragraph console, Action startProcess, Action stopProcess, Options options)
        {
            _dispatcher = Application.Current.Dispatcher;
            _console = console;
            _startProcess = startProcess;
            _stopProcess = stopProcess;
            _options = options;

            TestOk = null;
            VersionOk = null;
            BuildOk = null;
            UploadOk = null;
        }

        public bool? TestOk
        {
            get => _testOk;
            private set => SetProperty(ref _testOk, value);
        }
        public bool? VersionOk
        {
            get => _versionOk;
            private set => SetProperty(ref _versionOk, value);
        }
        public bool? BuildOk
        {
            get => _buildOk;
            private set => SetProperty(ref _buildOk, value);
        }
        public bool? UploadOk
        {
            get => _uploadOk;
            private set => SetProperty(ref _uploadOk, value);
        }

        public bool CanBuild()
        {
            return _solutionBrowser != null;
        }
        public void SetSolutionBrowser(SolutionBrowser browser)
        {
            _solutionBrowser = browser;
        }

        private void CommonStart()
        {
            TestOk = null;
            VersionOk = null;
            BuildOk = null;
            UploadOk = null;

            _dispatcher.Invoke(() =>
            {
                _console.Inlines.Clear();
                _startProcess();
            });
        }
        private void CommonFinish() => _dispatcher.Invoke(_stopProcess);

        public void Restart(ReleaseItem item)
        {
            _releaseItem = item;
            Task.Run(new Action(RunCommonBuild));
        }
        public void Setup()
        {
            Task.Run(new Action(RunSetupBuild));
        }
        public void Build(UpdateType type)
        {
            _releaseItem = new ReleaseItem(_options.SolutionPath) { UpdateType = type };
            Store.StoreInstance.PushReleaseItem(_releaseItem);
            Task.Run(new Action(RunCommonBuild));
        }

        private void RunSetupBuild()
        {
            CommonStart();

            try
            {
                PrepareTemp();
                CreateProjectCollection();
                try
                {
                    LoggerWriteNewLine(Environment.NewLine + "Begin Writing Setup...");
                    LoggerWriteEmpty();

                    var setupProject = ConfigurateProject(_options.SetupProject, "SetupProject", out var unused);

                    setupProject.Build("Build");
                    if (_buildFailed)
                    {
                        LoggerWriteNewLine("\t Setup Build Failed...");
                        BuildOk = false;
                        UploadOk = false;
                        return;
                    }

                    string setupPath = _lastBuildPath;
                    var bootStrapper = ConfigurateProject(_options.SetupBootstrapper, "SetupBootstrapper", out var projectInfo);
                    
                    string compressionPath = Path.Combine(Path.GetDirectoryName(projectInfo.FullName) ?? throw new InvalidOperationException(), "file.lz");
                    string zipFile = Path.Combine(_tempPath, "file.zip");

                    LoggerWriteEmpty();
                    LoggerWriteNewLine("Creating Zip...");

                    foreach (var file in Directory.EnumerateFiles(setupPath, "*.*", SearchOption.AllDirectories).ToArray())
                    {
                        var ext = Path.GetExtension(file);

                        switch (ext)
                        {
                            case ".xml":
                            case ".nuspec":
                            case ".pdb":
                                File.Delete(file);
                                break;
                        }
                    }
                    //ZipFile.CreateFromDirectory(setupPath, zipFile, CompressionLevel.NoCompression, false);

                    using (ZipFile file = new ZipFile(zipFile))
                    {
                        file.CompressionMethod = CompressionMethod.None;
                        file.CompressionLevel = CompressionLevel.None;
                        file.AddSelectedFiles("*.*", setupPath, string.Empty,true);
                        file.Save();
                    }

                    AppendLoggerWrite("Completed" + Environment.NewLine);

                    LoggerWriteNewLine("Compressing...");
                    using (var sourceStream = new FileStream(zipFile, FileMode.Open))
                        using (var targetStream = new FileStream(compressionPath, FileMode.Create))
                            using (var compressstream = new LZipStream(targetStream, CompressionMode.Compress))
                                sourceStream.CopyTo(compressstream);
                    AppendLoggerWrite("Completed" + Environment.NewLine);
                    LoggerWriteNewLine(Environment.NewLine);

                    if (!bootStrapper.Build("Build") && _buildFailed)
                    {
                        LoggerWriteNewLine("\t Bootstrapper Build Failed...");
                        BuildOk = false;
                        UploadOk = false;
                        return;
                    }

                    BuildOk = true;
                    LoggerWriteNewLine(Environment.NewLine + "BootStrapper Build Conmpled" + Environment.NewLine);
                    LoggerWriteEmpty();
                    LoggerWriteNewLine("Begin Upload..." + Environment.NewLine);

                    string exe = Directory.EnumerateFiles(_lastBuildPath, "*.exe").Single();
                    if (!File.Exists(exe))
                    {
                        LoggerWriteNewLine("\tBootStrapper Exe not Found...");
                        UploadOk = false;
                        return;
                    }

                    LoggerWriteNewLine("\tGetting Release...  ");
                    GitHubClient client = CreateClient();

                    var rep = client.Repository.Get(_options.GitHubName, _options.GitHubRepository).Result;
                    var release = client.Repository.Release.GetAll(rep.Id).Result.FirstOrDefault(r => r.TagName == "s1.0");

                    if (release == null)
                    {
                        var diag = new InputDialog {AllowCancel = true, MainText = "Body", InstructionText = "Beschreibung des Releases"};
                        if (diag.ShowDialog() == false)
                        {
                            UploadOk = false;
                            LoggerWriteNewLine("\tBody Input Canceled...");
                            return;
                        }

                        release = client.Repository.Release.Create(rep.Id, new NewRelease("s1.0") {Name = "Setup", Body = diag.Result}).Result;
                    }
                    AppendLoggerWrite("Conmpled" + Environment.NewLine);

                    LoggerWriteNewLine("\tUploading...   ");
                    foreach (var asset in release.Assets)
                        client.Repository.Release.DeleteAsset(rep.Id, asset.Id).Wait();

                    ReleaseAsset uploadAsset;
                    using (var stream = new FileStream(exe, FileMode.Open))
                        uploadAsset = client.Repository.Release.UploadAsset(release, new ReleaseAssetUpload(Path.GetFileName(exe), "application/vnd.microsoft.portable-executable", stream, TimeSpan.FromMinutes(30))).Result;

                    if (uploadAsset == null)
                    {
                        AppendLoggerWrite("Failed" + Environment.NewLine);
                        UploadOk = false;
                        return;
                    }

                    AppendLoggerWrite("Conmpled" + Environment.NewLine);

                    LoggerWriteNewLine("Upload Conmpled" + Environment.NewLine);
                    UploadOk = true;
                    LoggerWriteNewLine("Setup Build Conmpled");
                }
                catch (Exception e)
                {
                    LoggerWriteEmpty();
                    LoggerWriteNewLine("Error on Build:");
                    LoggerWriteNewLine("\t" + e);

                    if (BuildOk == null)
                        BuildOk = false;
                    UploadOk = false;
                }
                finally
                {
                    DisposeCollection();
                }
            }
            finally
            {
                CommonFinish();
            }
        }
        private void RunCommonBuild()
        {
            if(_releaseItem.Completed) return;

            CommonStart();

            try
            {
                try
                {
                    PrepareTemp();
                    CreateProjectCollection();

                    try
                    {
                        LoggerWriteNewLine(Environment.NewLine + "Begin Writing App...");
                        LoggerWriteEmpty();

                        if (!_releaseItem.Test)
                        {
                            List<Tuple<string, string>> testPaths = new List<Tuple<string, string>>();
                            foreach (var proj in from p in _options.Tests where p.Value select p.Key)
                            {

                                var project = ConfigurateProject(proj, Path.GetFileNameWithoutExtension(proj), out var unused);
                                testPaths.Add(Tuple.Create(_lastBuildPath, project.GetProperty("AssemblyName").EvaluatedValue));
                                if (project.Build("Build")) continue;
                                if (!_buildFailed) continue;

                                AppendLoggerWrite("\t Test Build Failed..." + Environment.NewLine);

                                TestOk = false;
                                VersionOk = false;
                                BuildOk = false;
                                UploadOk = false;
                                return;
                            }

                            if (!RunTests(testPaths))
                            {
                                LoggerWriteEmpty();
                                LoggerWriteNewLine(Environment.NewLine + "Writing App Failed...");

                                return;
                            }

                            _releaseItem.Test = true;
                            Store.StoreInstance.SaveContainer();
                        }
                        else
                        {
                            LoggerWriteNewLine("Test Skipped" + Environment.NewLine);
                            TestOk = true;
                        }

                        if (!_releaseItem.Version)
                        {
                            LoggerWriteEmpty();
                            LoggerWriteNewLine("Update version...");

                            string[] project = _options.Application.Where(p => p.Value).Select(p => p.Key).ToArray();
                            VersionUpdater.UpdateVersion(_releaseItem.FullPath, _releaseItem.UpdateType,
                                new ProjectSortDescription {ProjectsAllowedForVersionModification = project, ProjectsToExclude = new string[0], ProjectsToForce = project.ToArray()});

                            AppendLoggerWrite("Ok");
                            VersionOk = true;

                            _releaseItem.Version = true;
                            Store.StoreInstance.SaveContainer();
                        }
                        else
                        {
                            LoggerWriteNewLine("Version Skipped" + Environment.NewLine);
                            VersionOk = true;
                        }

                        LoggerWriteEmpty();
                        LoggerWriteNewLine("Build Application...");

                        const string path = "Application";

                        var project1 = ConfigurateProject(_options.ApplicationProject, path, out var unused2);

                        project1.Build("Build");
                        if (_buildFailed)
                        {
                            LoggerWriteNewLine("\t Application Build Failed..." + Environment.NewLine);
                            BuildOk = false;
                            UploadOk = false;
                            return;
                        }
                        BuildOk = true;

                        LoggerWriteEmpty();
                        LoggerWriteNewLine("Creating Zip...");

                        foreach (var file in Directory.EnumerateFiles(_lastBuildPath, "*.*", SearchOption.AllDirectories).ToArray())
                        {
                            var ext = Path.GetExtension(file);

                            switch (ext)
                            {
                                case ".xml":
                                case ".nuspec":
                                    File.Delete(file);
                                    break;
                            }
                        }

                        string zipFile = Path.Combine(_tempPath, "Release.zip");
                        using (ZipFile file = new ZipFile(zipFile))
                        {
                            file.CompressionMethod = CompressionMethod.Deflate;
                            file.CompressionLevel = CompressionLevel.Level9;
                            file.AddSelectedFiles("*.*", _lastBuildPath, string.Empty, true);
                            file.Save();
                        }

                        AppendLoggerWrite("Completed");

                        LoggerWriteEmpty();
                        LoggerWriteNewLine("Uploading Release...");

                        var client = CreateClient();
                      
                        string exe = Directory.EnumerateFiles(_lastBuildPath, "*.exe").Single();
                        var def = AssemblyDefinition.ReadAssembly(exe);

                        string version = "v" + def.Name.Version;
                        string description = GetDescription();

                        var newRelease = new NewRelease(version) { Body = description, Name = version};
                        var release = client.Repository.Release.Create(_options.GitHubName, _options.GitHubRepository, newRelease).Result;
                        using (var stream = new FileStream(zipFile, FileMode.Open))
                            client.Repository.Release.UploadAsset(release, new ReleaseAssetUpload("Release.zip", "application/zip", stream, TimeSpan.FromHours(2))).Wait();

                        UploadOk = true;
                        AppendLoggerWrite("Completed");

                        LoggerWriteEmpty();
                        LoggerWriteNewLine("Finished");
                    }
                    catch (Exception e)
                    {
                        Exception e2 = e;
                        if (e is AggregateException e3)
                            e2 = e3.InnerExceptions.First();

                        LoggerWriteEmpty();
                        LoggerWriteNewLine("Error on Build:");
                        LoggerWriteNewLine("\t" + e2);

                        if (TestOk == null)
                            TestOk = false;
                        if (VersionOk == null)
                            VersionOk = false;
                        if (BuildOk == null)
                            BuildOk = false;
                        UploadOk = false;
                    }
                }
                finally
                {
                    DisposeCollection();
                }
            }
            finally
            {
                CommonFinish();
            }
        }

        private bool RunTests(IEnumerable<Tuple<string, string>> locations)
        {
            LoggerWriteNewLine("Beginn Testing..." + Environment.NewLine);
            LoggerWriteNewLine(Environment.NewLine);

            var copions = new TestOptions(locations.Select(t => Path.Combine(t.Item1, t.Item2 + ".dll")));

            var runner = new TestRunner(TestEngineActivator.CreateInstance(), copions, new ExtendedTextWrapper(new TestPersonalLogger(s => LoggerWriteNewLine("\t" + s))));
            var erg = runner.Execute();
            if (erg.Summary.ErrorCount != 0 || erg.Summary.UnexpectedError)
            {
                TestOk = false;
                LoggerWriteNewLine(Environment.NewLine);
                LoggerWriteNewLine("End Testing... Fail");

                return false;
            }

            LoggerWriteNewLine(Environment.NewLine);
            LoggerWriteNewLine("End Testing...");
            TestOk = true;
            return true;
        }
        private string GetDescription()
        {
            string filePath = Path.Combine(_lastBuildPath, "ChangeLog.txt");
            if (!File.Exists(filePath)) return string.Empty;

            try
            {
                StringBuilder realText = new StringBuilder();
                string text = File.ReadAllText(filePath).Trim();
                string[] segements = text.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                bool isAdded = false;

                foreach (var segemnt in segements)
                {
                    if (segemnt.StartsWith("Version"))
                    {
                        if (isAdded) break;
                        continue;
                    }

                    realText.Append(segemnt.Trim());
                    isAdded = true;
                }

                return realText.ToString();
            }
            catch (IOException)
            {
                return string.Empty;
            }
        }

        private void PrepareTemp()
        {
            _tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (Directory.Exists(_tempPath))
                Directory.Delete(_tempPath, true);
        }
        private Project ConfigurateProject(string name, string pathName, out ProjectInfo projectInfo)
        {
            var project = GetProjectByName(_projectCollection, name, out projectInfo);
            _lastBuildPath = Path.Combine(_tempPath, pathName);
            SetOutputDirectory(project, _lastBuildPath);

            return project;
        }
        private GitHubClient CreateClient()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("Auto_Releaser", "1.0"), new InMemoryCredentialStore(new Credentials(_options.GitHubName, InputDialog.ShowDialog("Passwort"))));

            var httpClient = client.Connection.GetType().GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(client.Connection);
            HttpClient htclient = (HttpClient)httpClient?.GetType().GetField("_http", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(httpClient);
            if (htclient != null) htclient.Timeout = TimeSpan.FromHours(1);
            else throw new InvalidOperationException("HTTP Client Timeout not setable.");

            return client;
        }

        private void CreateProjectCollection()
        {
            _projectCollection = new ProjectCollection { IsBuildEnabled = true };
            
            _projectCollection.RegisterLogger(new PersonalLogger(LoggerWriteNewLine, b => _buildFailed = b));
            var logger = new Microsoft.Build.Logging.FileLogger { Verbosity = LoggerVerbosity.Normal };

            _projectCollection.RegisterLogger(logger);

            foreach (var pi in _solutionBrowser.ProjectInfoList.Where(pi => pi.ProjectTypeInfo.ProjectType == ProjectType.CSharpProject))
                _projectCollection.LoadProject(pi.FullName);
        }
        private void DisposeCollection()
        {
            _projectCollection.UnloadAllProjects();
            _projectCollection.Dispose();
            _projectCollection = null;
        }
        private Project GetProjectByName(ProjectCollection collection, string name, out ProjectInfo projectInfo)
        {
            projectInfo = _solutionBrowser.ProjectInfoList.Where(pi2 => pi2.ProjectTypeInfo.ProjectType == ProjectType.CSharpProject).First(pi1 => pi1.ProjectName == name);
            var pi = projectInfo;
            return collection.LoadedProjects.First(p => p.FullPath == pi.FullName);
        }
        private static void SetOutputDirectory(Project project, string path)
        {
            project.SetProperty("Configuration", "Release");
            project.SetProperty("Platform", "AnyCPU");
            project.ReevaluateIfNecessary();

            project.SetProperty("OutputPath", path);
            project.ReevaluateIfNecessary();
        }

        private Run _currentRun;
        private void LoggerWriteNewLine(string message)
        {
            _dispatcher.Invoke(() =>
            {
                _currentRun = new Run {Text = message};
                _console.Inlines.Add(_currentRun);
            });
        }
        private void AppendLoggerWrite(string message)
        {
            const int baseLenght = 120;

            _dispatcher.Invoke(() =>
            {
                int effectivLenght = baseLenght - (_currentRun.Text.Length + message.Length);
                _currentRun.Text += message.PadLeft(effectivLenght);
            });
        }
        private void LoggerWriteEmpty()
        {
            var temp = Environment.NewLine;
            LoggerWriteNewLine(string.Concat(temp, temp));
        }
    }
}