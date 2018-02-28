using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using JetBrains.Annotations;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter.Rule
{
    [PublicAPI]
    public class XamlToXps
    {
        /// <summary>
        /// Holds an internal copy of the Loaded Xaml
        /// </summary>
        private IDocumentPaginatorSource _flowDocument;
        /// <summary>
        /// This Dictionary is used to perform string replacements in the source Xaml, 
        /// I use this f.ex. to replace an external default reference to an actual XML 
        /// file location
        /// </summary>
        private Dictionary<string, string> _stringReplacement;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlToXps"/> class.
        /// </summary>
        public XamlToXps()
        {
            _stringReplacement = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the string replacement dictionary later to be used for string 
        /// replacements in the Xaml source.
        /// </summary>
        /// <value>The string replacement dictionary.</value>
        public Dictionary<string, string> StringReplacement
        {
            set => _stringReplacement = value;
            get => _stringReplacement;
        }

        /// <summary>
        /// Loads the source Xaml using a file location.
        /// At this stage there will be automatic string replacement.
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        public void LoadXaml(string xamlFileName)
        {
            using (FileStream inputStream = File.OpenRead(xamlFileName))
                LoadXaml(inputStream);
        }
        /// <summary>
        /// Loads the source Xaml using a Stream.
        /// At this stage there will be automatic string replacement.
        /// </summary>
        /// <param name="xamlFileStream">The xaml file stream.</param>
        public void LoadXaml(Stream xamlFileStream)
        {
            ParserContext pc = new ParserContext
                               {
                                   BaseUri = new Uri(Environment.CurrentDirectory + "/")
                               };
            object newDocument = XamlReader.Load(ReplaceStringsInXaml(xamlFileStream), pc);

            if (newDocument == null)
                throw new Exception("Invalid Xaml, could not be parsed");

            if (newDocument is IDocumentPaginatorSource)
                LoadXaml(newDocument as IDocumentPaginatorSource);
        }
        /// <summary>
        /// Loads the source Xaml in the form of a complete FlowDocument. 
        /// At this stage there is no automatic string replacement.
        /// </summary>
        /// <param name="flowDocument">The flow document.</param>
        public void LoadXaml(IDocumentPaginatorSource flowDocument)
        {
            _flowDocument = flowDocument;
            FlushDispatcher(flowDocument as DispatcherObject);
        }

        public void Save(Stream stream)
        {
            FlowDocument flowDocument = _flowDocument as FlowDocument;
            flowDocument.Dispatcher.Invoke(() =>
                                           {
                                               using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
                                               {
                                                   
                                                   var packUri = new Uri("pack://temp.xps");
                                                   PackageStore.RemovePackage(packUri);
                                                   PackageStore.AddPackage(packUri, package);

                                                   var xpsDocument = new XpsDocument(package, CompressionOption.SuperFast, packUri.ToString());

                                                   var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                                                   
                                                   new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false)
                                                       .SaveAsXaml(paginator);
                                               }


                                               //TextRange range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);

                                               //range.Save(stream, DataFormats.XamlPackage);
                                           });
        }

        /// <summary>
        /// Deletes the old output file if it exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void DeleteOldFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        /// <summary>
        /// Replaces the Key Value Pairs in the Xaml source, this is an other way of data 
        /// binding, but I prefere the build-in way.
        /// </summary>
        /// <param name="xamlFileStream">The xaml file stream.</param>
        /// <returns></returns>
        private Stream ReplaceStringsInXaml(Stream xamlFileStream)
        {
            string rawXamlText;
            xamlFileStream.Seek(0, SeekOrigin.Begin);
            using (StreamReader streamReader = new StreamReader(xamlFileStream))
                rawXamlText = streamReader.ReadToEnd();
            foreach (KeyValuePair<string, string> keyValuePair in _stringReplacement)
                rawXamlText = rawXamlText.Replace(keyValuePair.Key, keyValuePair.Value);
            return new MemoryStream(new UTF8Encoding().GetBytes(rawXamlText));
        }

        /// <summary>
        /// Have to figure out what this does and why it works, but not including 
        /// this and calling it wil actually fail the databinding process
        /// </summary>
        /// <param name="ctx"></param>
        private static void FlushDispatcher(DispatcherObject ctx) => FlushDispatcher(ctx?.Dispatcher ?? ((Window)CommonApplication.Current.MainWindow?.TranslateForTechnology())?.Dispatcher);

        /// <summary>
        /// Have to figure out what this does and why it works, but not including 
        /// this and calling it wil actually fail the databinding process
        /// </summary>
        /// <param name="ctx"></param>
        private static void FlushDispatcher(Dispatcher ctx)
        {
            FlushDispatcher(ctx, DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Have to figure out what this does and why it works, but not including 
        /// this and calling it wil actually fail the databinding process
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="priority"></param>
        private static void FlushDispatcher(Dispatcher ctx, DispatcherPriority priority)
        {
            ctx.Invoke(priority, new DispatcherOperationCallback(delegate { return null; }), null);
        }
    }
}