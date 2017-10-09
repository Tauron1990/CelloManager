using System.Collections.Generic;

namespace AutoReleaser.Builder
{
    public class TestOptions
    {
        public TestOptions(IEnumerable<string> inputFiles)
        {
            InputFiles = inputFiles;
        }
        
        public IEnumerable<string> InputFiles { get; }
    }
}