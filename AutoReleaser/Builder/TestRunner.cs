using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using NUnit.Common;
using NUnit.ConsoleRunner;
using NUnit.ConsoleRunner.Utilities;
using NUnit.Engine;

namespace AutoReleaser.Builder
{
    public class TestRunner
    {
        public static readonly int OK = 0;
        // ReSharper disable UnusedMember.Global
        public static readonly int InvalidArg = -1;
        public static readonly int InvalidAssembly = -2;
        public static readonly int InvalidTestFixture = -4;
        public static readonly int UnexpectedError = -100;
        // ReSharper restore UnusedMember.Global
        private readonly ITestEngine _engine;
        private readonly TestOptions _options;
        private readonly ITestFilterService _filterService;
        private readonly ExtendedTextWriter _outWriter;

        public TestRunner(ITestEngine engine, TestOptions options, ExtendedTextWriter writer)
        {
            _engine = engine;
            _options = options;
            _outWriter = writer;
            
            _filterService = _engine.Services.GetService<ITestFilterService>();
        }

        public ResultReporter Execute()
        {
            DisplayRuntimeEnvironment(_outWriter);
            DisplayTestFiles();

            TestPackage package = MakeTestPackage(_options);

            TestFilter testFilter = CreateTestFilter();

            return RunTests(package, testFilter);
        }

        private void DisplayTestFiles()
        {
            _outWriter.WriteLine(ColorStyle.SectionHeader, "Test Files");
            foreach (string inputFile in _options.InputFiles)
                _outWriter.WriteLine(ColorStyle.Default, "    " + Path.GetFileName(inputFile));
            _outWriter.WriteLine();
        }

        private ResultReporter RunTests(TestPackage package, TestFilter filter)
        {
            string labelsOption = "ON";
            XmlNode resultNode = null;
            NUnitEngineException nunitEngineException = null;
            try
            {

                using (ITestRunner runner = _engine.GetRunner(package))
                {

                    TestEventHandler testEventHandler = new TestEventHandler(_outWriter, labelsOption);
                    resultNode = runner.Run(testEventHandler, filter);

                }

            }
            catch (NUnitEngineException ex)
            {
                nunitEngineException = ex;
            }

            if (resultNode != null)
            {
                ResultReporter resultReporter = new ResultReporter(resultNode,  _outWriter, new ConsoleOptions());
                resultReporter.ReportResults();
                return resultReporter;
            }
            if (nunitEngineException != null)
                _outWriter.WriteLine(ColorStyle.Error, nunitEngineException.Message);
            return null;
        }

        private void DisplayRuntimeEnvironment(ExtendedTextWriter outWriter)
        {
            outWriter.WriteLine(ColorStyle.SectionHeader, "Runtime Environment");
            outWriter.WriteLabelLine("   OS Version: ", GetOsVersion());
            outWriter.WriteLabelLine("  CLR Version: ", Environment.Version.ToString());
            outWriter.WriteLine();
        }

        private static string GetOsVersion()
        {
            OperatingSystem osVersion = Environment.OSVersion;
            string str1 = osVersion.ToString();

            if (osVersion.Platform != PlatformID.Unix) return str1;

            IntPtr num = Marshal.AllocHGlobal(8192);
            if (uname(num) == 0)
            {
                string str2 = Marshal.PtrToStringAnsi(num);
                if (str2 != null && str2.Equals("Darwin"))
                    str2 = "MacOSX";
                str1 = $"{ str2} {osVersion.Version} {osVersion.ServicePack}";
            }
            Marshal.FreeHGlobal(num);
            return str1;
        }

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);


        private static TestPackage MakeTestPackage(TestOptions options)
        {
            TestPackage testPackage = new TestPackage(options.InputFiles.ToList());
            testPackage.AddSetting("ProcessModel", "InProcess");
            testPackage.AddSetting("DomainUsage", "Single");
            testPackage.AddSetting("DisposeRunners",  true);
            testPackage.AddSetting("LoadUserProfile",  true);
            testPackage.AddSetting("SkipNonTestAssemblies",  true);
            testPackage.AddSetting("MaxAgents",  1);
            testPackage.AddSetting("NumberOfTestWorkers",  1);
            //testPackage.AddSetting("PrivateBinPath", AppDomain.CurrentDomain.BaseDirectory);
            return testPackage;
        }

        private TestFilter CreateTestFilter()
        {
            ITestFilterBuilder testFilterBuilder = _filterService.GetTestFilterBuilder();

            return testFilterBuilder.GetFilter();
        }
    }
}