using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace AutoReleaser.SolutionLoader.Configuration
{
    public enum ExportFileFormat
    {
        PlainText
    }

    #region AssemblyVersionTypeSelection

    /// <summary>
    ///     Class with order and selection of assembly version types that are
    ///     exported.
    /// </summary>
    [Serializable]
    public class AssemblyVersionTypeSelection
    {
        /// <summary>
        ///     Default array of <c>AssemblyVersionTypeSelection</c> with all types selected.
        /// </summary>
        public static readonly AssemblyVersionTypeSelection[] DefaultSelection
            =
            {
                new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyVersion),
                new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyFileVersion),
                new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyInformationalVersion)
            };

        private AssemblyVersionType _assemblyVersionType;

        private bool _isSelected;

        #region Constructors

        /// <summary>
        ///     Creates empty <c>AssemblyVersionTypeSelection</c> object.
        /// </summary>
        public AssemblyVersionTypeSelection()
        {
            _isSelected = true;
        }

        /// <summary>
        ///     Creates <c>AssemblyVersionTypeSelection</c> object with
        ///     <c>AssemblyVersionType</c> provided.
        /// </summary>
        /// <param name="assemblyVersionType">
        ///     <c>AssemblyVersionType</c> to initialize with.
        /// </param>
        public AssemblyVersionTypeSelection(AssemblyVersionType assemblyVersionType) : this()
        {
            _assemblyVersionType = assemblyVersionType;
        }

        #endregion // Constructors

        #region Public properties

        /// <summary>
        ///     Gets or sets <c>AssemblyVersionType</c> flag.
        /// </summary>
        [XmlText]
        public AssemblyVersionType AssemblyVersionType
        {
            get => _assemblyVersionType;
            set => _assemblyVersionType = value;
        }

        /// <summary>
        ///     Gets or sets flag indicating if current type is selected.
        /// </summary>
        [XmlAttribute]
        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        #endregion // Public properties
    }

    #endregion // AssemblyVersionTypeSelection

    #region ExportConfiguration

    /// <summary>
    ///     Configuration used for exporting.
    /// </summary>
    [Serializable]
    public class ExportConfiguration
    {
        #region Constructors

        public ExportConfiguration()
        {
            _assemblyVersionTypes = AssemblyVersionTypeSelection.DefaultSelection;
            _indentSubItems = true;
            _indentSubItemsBy = 1;
            _excludeNonversionableItems = false;
            _exportFileFormat = ExportFileFormat.PlainText;
            _csvSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            Debug.Assert(_assemblyVersionTypes != null && _assemblyVersionTypes.Length == 3);
        }

        #endregion // Constructors

        #region Public properties

        public AssemblyVersionTypeSelection[] AssemblyVersionTypes
        {
            get => _assemblyVersionTypes;
            set => _assemblyVersionTypes = value;
        }

        public bool IndentSubItems
        {
            get => _indentSubItems;
            set => _indentSubItems = value;
        }

        public int IndentSubItemsBy
        {
            get => _indentSubItemsBy;
            set => _indentSubItemsBy = value;
        }

        public bool ExcludeNonversionableItems
        {
            get => _excludeNonversionableItems;
            set => _excludeNonversionableItems = value;
        }

        public ExportFileFormat ExportFileFormat
        {
            get => _exportFileFormat;
            set => _exportFileFormat = value;
        }

        public string CsvSeparator
        {
            get => _csvSeparator;
            set => _csvSeparator = value;
        }

        #endregion // Public properties

        #region Private fields

        private AssemblyVersionTypeSelection[] _assemblyVersionTypes;

        private bool _indentSubItems;

        private int _indentSubItemsBy;

        private bool _excludeNonversionableItems;

        private ExportFileFormat _exportFileFormat;

        private string _csvSeparator;

        #endregion // Private fields
    }

    #endregion // ExportConfiguration
}