using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Ionic.Zip;
using Ionic.Zlib;
using Mono.Cecil;
using Octokit;
using UpdateUploader.Properties;
using FileMode = System.IO.FileMode;

namespace UpdateUploader
{
    public partial class Form1 : Form
    {
        private readonly Settings _settings = Settings.Default;
        private readonly System.Timers.Timer _timer;

        public Form1()
        {
            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = 1000,
                SynchronizingObject = this,
            };
            _timer.Elapsed += Save;

            InitializeComponent();
        }

        private void BlockUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(BlockUI));
                return;
            }

            _uploadButton.Enabled = false;
            _loadChangelog.Enabled = false;
            _descriptionBox.Enabled = false;
            _loadVersion.Enabled = false;
            _versionBox.Enabled = false;
            _nameBox.Enabled = false;
            _openApplicationPath.Enabled = false;
            _applicationPathBox.Enabled = false;
            _userNameBox.Enabled = false;
            _repositoryBox.Enabled = false;
        }
        private void UnblockUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UnblockUI));
                return;
            }

            _uploadButton.Enabled = true;
            _loadChangelog.Enabled = true;
            _descriptionBox.Enabled = true;
            _loadVersion.Enabled = true;
            _versionBox.Enabled = true;
            _nameBox.Enabled = true;
            _openApplicationPath.Enabled = true;
            _applicationPathBox.Enabled = true;
            _userNameBox.Enabled = true;
            _repositoryBox.Enabled = true;
        }

        private void StartProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StartProgress));
                return;
            }

            BlockUI();
            _upLoadProgress.Style = ProgressBarStyle.Marquee;
        }
        private void StopProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(StopProgress));
            }

            _upLoadProgress.Style = ProgressBarStyle.Blocks;
            UnblockUI();
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            StartProgress();
            Task.Run(new Action(StartUploadProgress)).ContinueWith(t => StopProgress());
        }
        private void OpenApplicationPath_Click(object sender, EventArgs e)
        {
            if (_appPathDialog.ShowDialog(this) == DialogResult.OK)
                _applicationPathBox.Text = _appPathDialog.SelectedPath;
        }
        private void LoadVersion_Click(object sender, EventArgs e)
        {
            string appPath = _applicationPathBox.Text;

            Task.Run(() =>
            {
                string app = GetExecutebelFile(appPath, out var def);
                if(!File.Exists(app)) return;

                Invoke(new Action(() => _versionBox.Text = "v" + def.Name.Version));
            });
        }
        private void LoadChangeLog_Click(object sender, EventArgs e)
        {
            string basePath = _applicationPathBox.Text;

            Task.Run(() =>
            {
                if(!Directory.Exists(basePath)) return;

                string filePath = Path.Combine(basePath, "ChangeLog.txt");
                if(!File.Exists(filePath)) return;

                try
                {
                    StringBuilder realText = new StringBuilder();
                    string text = File.ReadAllText(filePath).Trim();
                    string[] segements = text.Split(new[]{ ':' }, StringSplitOptions.RemoveEmptyEntries);
                    bool isAdded = false;
                    
                    foreach (var segemnt  in segements)
                    {
                        if (segemnt.StartsWith("Version"))
                        {
                            if(isAdded) break;
                            continue;
                        }

                        realText.Append(segemnt.Trim());
                        isAdded = true;
                    }

                    Invoke(new Action(() => _descriptionBox.Text = realText.ToString()));
                }
                catch (IOException)
                {
                }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _applicationPathBox.Text = _settings.AppPath;
            _userNameBox.Text = _settings.UserName;
            _repositoryBox.Text = _settings.RepoName;
        }


        private void StartUploadProgress()
        {
            string userName = GetInvokeText(_userNameBox);
            string repo = GetInvokeText(_repositoryBox);

            string appPath = GetInvokeText(_applicationPathBox);
            string nametext = GetInvokeText(_nameBox);
            string versiontext = GetInvokeText(_versionBox);
            string descriptionText = GetInvokeText(_descriptionBox);

            if (Check(() => Directory.Exists(appPath), "Anwendungs verzeichniss muss Existieren!")) return;
            if (Check(() => !string.IsNullOrWhiteSpace(nametext), "Ein Name muss angegeben werden!")) return;
            if (Check(() => !string.IsNullOrWhiteSpace(versiontext), "Ein Version Tag muss angegeben werden")) return;

            try
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("Update_Uploader", "1.0"))
                {
                    Connection = {Credentials = new Credentials("b65b7c067bc5296df71ea2b439090410d1e2fcf2")}
                };


                var newRelease = new NewRelease(versiontext)
                {
                    Draft = false,
                    Body = descriptionText,
                    Name = nametext
                };

                var release = client.Repository.Release.Create(userName, repo, newRelease).Result;
                //string file = CreateAsset(appPath);

                //using (FileStream stream = new FileStream(file, FileMode.Open))
                using (MemoryStream stream = new MemoryStream())
                {
                    CreateAsset(appPath, stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    client.Repository.Release.UploadAsset(release, new ReleaseAssetUpload {ContentType = "application/zip", FileName = "Release.zip", RawData = stream}).Wait();
                }

                //File.Delete(file);
            }
            catch (Exception e) when (e is IOException || e is ApiException || e is AggregateException)
            {
                string messege;

                if (e is AggregateException aex)
                    messege = aex.InnerExceptions[0].Message;
                else
                    messege = e.Message;


                MessageBox.Show(messege, "Fehler", MessageBoxButtons.OK);
            }
        }

        private void CreateAsset(string basepath, MemoryStream stream)
        {
            string[] blacklist = { ".nuspec", ".xml" };

            //string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            //if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            using (ZipFile file = new ZipFile())
            {
                file.CompressionLevel = CompressionLevel.BestCompression;
                file.CompressionMethod = CompressionMethod.BZip2;
                file.AddDirectory(basepath);
                
                file.RemoveEntries((from entry in file
                        let name = entry.FileName
                        where blacklist.Any(s => name.Trim().EndsWith(s))
                        select entry)
                    .ToList());

                file.Save(stream);
            }

           // return tempPath;
        }
        private bool Check(Func<bool> expression, string messege)
        {
            if(expression()) return false;

            MessageBox.Show(messege, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return true;
        }
        private string GetInvokeText(TextBoxBase text)
        {
            if (InvokeRequired) return Invoke(new Func<TextBoxBase, string>(GetInvokeText), text) as string;

            return text.Text;
        }
        private string GetExecutebelFile(string basePath, out AssemblyDefinition def)
        {
            def = null;

            if (!Directory.Exists(basePath)) return string.Empty;

            string[] files = Directory.GetFiles(basePath, "*.exe");

            foreach (var file in files)
            {
                try
                {

                    def = AssemblyDefinition.ReadAssembly(file);
                    if (def != null) return file;
                }
                catch (Exception e) when(e is ArgumentException || e is BadImageFormatException)
                {
                }

            }

            return string.Empty;
        }
        private void TextChangedImpl(object sender, EventArgs e)
        {
            ResetTimer();
        }
        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
        private void Save(object sender, ElapsedEventArgs args)
        {

            _settings.AppPath = _applicationPathBox.Text;
            _settings.RepoName = _repositoryBox.Text;
            _settings.UserName = _userNameBox.Text;
            _settings.Save();
        }
    }
}
