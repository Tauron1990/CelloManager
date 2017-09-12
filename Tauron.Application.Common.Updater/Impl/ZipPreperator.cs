using System;
using Ionic.Zip;
using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class ZipPreperator : IPreperator
    {
        private readonly string _preperationPath;

        public ZipPreperator(string preperationPath)
        {
            _preperationPath = preperationPath;
        }

        public event EventHandler<PreperationProgressEventArgs> PreperationInProgressEvent;

        public string Prepare(string path)
        {
            using (var file = ZipFile.Read(path))
            {
                file.ExtractProgress += FileOnExtractProgress;
                file.ExtractAll(_preperationPath, ExtractExistingFileAction.OverwriteSilently);
            }

            return _preperationPath;
        }

        private void FileOnExtractProgress(object sender, ExtractProgressEventArgs extractProgressEventArgs)
        {
            switch (extractProgressEventArgs.EventType)
            {
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                case ZipProgressEventType.Extracting_BeforeExtractEntry:
                    double percent = extractProgressEventArgs.EntriesTotal / 100d * extractProgressEventArgs.EntriesExtracted;

                    PreperationInProgressEvent?.Invoke(this, new PreperationProgressEventArgs(percent));
                    break;
            }
           
        }

        public void ExtractFiles(string source, string target)
        {
            using (var file = ZipFile.Read(source))
            {
                file.ExtractProgress += FileOnExtractProgress;
                file.ExtractAll(target, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
