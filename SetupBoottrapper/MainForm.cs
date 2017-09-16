using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Compressors.LZMA;
using CompressionMode = SharpCompress.Compressors.CompressionMode;

namespace SetupBoottrapper
{
    public partial class MainForm : Form
    {
        //private class ProgressTracker
        //{
        //    private readonly long _streamLeght;
        //    private readonly Action<int> _report;
        //    private int _step = 50;

        //    public ProgressTracker(long streamLeght, Action<int> report)
        //    {
        //        _streamLeght = streamLeght;
        //        _report = report;
        //    }

        //    public void SetProgress(long inSize)
        //    {
        //        if (_step == 50)
        //        {
        //            _report((int)((double)100 / _streamLeght * inSize));
        //            _step = 0;
        //        }
        //        else _step++;
        //    }
        //}

        private readonly string _tempLocation;

        public MainForm()
        {
            InitializeComponent();
            _tempLocation = Path.Combine(Path.GetTempPath(), "Tauron");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(new Action(DoWork));
        }

        private void DoWork()
        {
           try
            {
                using (var input = Assembly.GetExecutingAssembly().GetManifestResourceStream("SetupBoottrapper.file.lz"))
                {
                    if(input == null) throw new InvalidOperationException();
                    if(Directory.Exists(_tempLocation)) Directory.Delete(_tempLocation, true);

                    using (var stream = new MemoryStream())
                    {
                        //var tracker = new ProgressTracker(input.Length, i => Invoke(new Action<int>(ProgressChanged), i));

                        using (var decompress = new LZipStream(input, CompressionMode.Decompress, true))
                            decompress.CopyTo(stream);

                        stream.Seek(0, SeekOrigin.Begin);

                        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                            archive.ExtractToDirectory(_tempLocation);

                        //Invoke(new Action<int>(ProgressChanged), 100);
                    }
                }

                Process.Start(Path.Combine(_tempLocation, "Setup.exe"))?.WaitForExit();
            }
            finally
            {
                Directory.Delete(_tempLocation, true);
            }

            Invoke(new Action(Close));
        }

        //private void ProgressChanged(int per)
        //{
        //    progressBar.Value = per;
        //}
    }
}
