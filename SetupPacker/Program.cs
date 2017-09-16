using System;
using System.IO;
using System.IO.Compression;
using SetupPacker.Helper;
using SharpCompress.Compressors.LZMA;
using CompressionMode = SharpCompress.Compressors.CompressionMode;

namespace SetupPacker
{
    static class Program
    {
        private class ProgressTracker
        {
            private readonly long _streamLeght;
            private readonly Action<int> _report;
            private int _step = 50;

            public long InSize { get; private set; }
            public long OutSize { get; private set; }

            public ProgressTracker(long streamLeght, Action<int> report)
            {
                _streamLeght = streamLeght;
                _report = report;
            }

            public void SetProgress(long inSize, long outSize)
            {
                InSize = inSize;
                OutSize = outSize;

                if (_step == 50)
                {
                    _report((int) ((double) 100 / _streamLeght * inSize));
                    _step = 0;
                }
                else _step++;
            }
        }

        private static void Main()
        {
            var console = new ConsoleFormatter
            {
                ProgrammTitle = "Setup Packer",
                Title = "Zip Packageing"
            };
            
            console.Initialize();
            console.WriteLine();
            console.WriteLine("Creating Compressed Setup:");
            console.WriteLine();

            const string setupLoc = @"C:\Users\Alexander\Documents\Visual Studio 2015\Projects\CelloManager\Setup\bin\Release";
            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            const string target = @"C:\Users\Alexander\Documents\Visual Studio 2015\Projects\CelloManager\SetupBoottrapper\file.lz";

            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);

            try
            {

                console.AddListLabel("zip", "Create Zip:", 35);
                var tempFilePath = Path.Combine(tempPath, "temp.zip");
                ZipFile.CreateFromDirectory(setupLoc, tempFilePath, CompressionLevel.NoCompression, false);
                console.SetListLabel("zip", "Completed");

                console.AddListLabel("lzma", "Compressing:", 35);
                int ratio;
               // var encoder = new Encoder();
                using (var sourceStream = new FileStream(tempFilePath, FileMode.Open))
                {
                    var tracker = new ProgressTracker(sourceStream.Length, i => console.SetListLabel("lzma", i + "%", false));

                    using (var targetStream = new FileStream(target, FileMode.Create))
                    {
                        using (var compressstream = new LZipStream(targetStream, CompressionMode.Compress))
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            Copier.CopyStream(sourceStream, compressstream, l => tracker.SetProgress(l, targetStream.Length));
                        }
                    }

                    ratio = (int) ((double) 100 / tracker.InSize * tracker.OutSize);
                }
                console.SetListLabel("lzma", "Completed");
                console.WriteLine();
                console.WriteLine("Ratio: " + ratio + "%");
            }
            finally
            {
                Directory.Delete(tempPath, true);
            }

            console.WriteLine();
            console.Write("Creation Compled...");
            Console.ReadKey();
        }
    }
}