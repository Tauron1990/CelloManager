using System.Windows.Documents;

namespace Tauron.Application.CelloManager.Setup.Resources
{
    public sealed class ResourceWrapper
    {
        public string CommonNext => UIResources.CommonNext;

        public string CommonBack => UIResources.CommonBack;

        public string CommonCancel => UIResources.CommonCancel;

        public string CommonFinish => UIResources.CommonFinish;

        public string WizardTitleWerlcome => UIResources.WizardTitleWerlcome;

        public string WizardContentWelcome => UIResources.WizardContentWelcome;

        public string WizardTitleFinish => UIResources.WizardTitleFinish;

        public string WizardDescriptionFinish => UIResources.WizardDescriptionFinish;

        public string WizardContentFinishStartAppLabel => UIResources.WizardContentFinishStartAppLabel;

        public string WizardTitleProgress => UIResources.WizardTitleProgress;

        public string WizardDescriptionProgress => UIResources.WizardDescriptionProgress;

        private string LicenseText => UIResources.LicenseText;

        public string WizardTitleLicense => UIResources.WizardTitleLicense;

        public string WizardDescriptionLicense => UIResources.WizardDescriptionLicense;

        public FlowDocument LicenseTextDocument => new FlowDocument(new Paragraph(new Run(LicenseText){FontSize = 13}));

        public string WizardLicenseAcceptetLabel => UIResources.WizardLicenseAcceptetLabel;

        public string WizardTitleSelectFolder => UIResources.WizardTitleSelectFolder;

        public string WizardDescriptionSelectFolder => UIResources.WizardDescriptionSelectFolder;

        public string WizardSelectFolderCommonLabel => UIResources.WizardSelectFolderCommonLabel;

        public string WizardSelectFolderGroupHeader => UIResources.WizardSelectFolderGroupHeader;

        public string WizardSelectFolderBrowseLabel => UIResources.WizardSelectFolderBrowseLabel;

        public string WizardCreateShortcutLabel => UIResources.WizardCreateShortcutLabel;
    }
}