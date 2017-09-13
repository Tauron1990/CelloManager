using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Tauron.Application.CelloManager.Setup.Resources;

namespace Tauron.Application.CelloManager.Setup
{
    public sealed class PageContext : INotifyPropertyChanged
    {
        private readonly Regex _containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]");
        private bool? _startApp = true;
        private string _installLocation;
        private string _installMessege;
        private bool _isIndeterminate = true;
        private double _progressPercent;
        private bool _licenseAccepted;
        private bool _isInstallationLocationValid;
        private bool _createShortcut = true;

        public bool? StartApp
        {
            get => _startApp;
            set
            {
                if (value == _startApp) return;
                _startApp = value;
                OnPropertyChanged();
            }
        }

        public string InstallLocation
        {
            get => _installLocation;
            set
            {
                if (value == _installLocation) return;
                IsInstallationLocationValid = IsValidFilename(ref value);
                _installLocation = value;
                OnPropertyChanged();
            }
        }

        public string InstallMessege
        {
            get => _installMessege;
            set
            {
                if (value == _installMessege) return;
                _installMessege = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                if (value == _isIndeterminate) return;
                _isIndeterminate = value;
                OnPropertyChanged();
            }
        }

        public double ProgressPercent
        {
            get => _progressPercent;
            set
            {
                if (value.Equals(_progressPercent)) return;
                _progressPercent = value;
                OnPropertyChanged();
            }
        }

        public bool LicenseAccepted
        {
            get => _licenseAccepted;
            set
            {
                if (value == _licenseAccepted) return;
                _licenseAccepted = value;
                OnPropertyChanged();
            }
        }

        public bool IsInstallationLocationValid
        {
            get => _isInstallationLocationValid;
            set
            {
                if (value == _isInstallationLocationValid) return;
                _isInstallationLocationValid = value;
                OnPropertyChanged();
            }
        }

        public bool CreateShortcut
        {
            get => _createShortcut;
            set
            {
                if (value == _createShortcut) return;
                _createShortcut = value;
                OnPropertyChanged();
            }
        }

        public PageContext()
        {
            InstallMessege = UIResources.PrograssInitialMessage;
            InstallLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Tauron Cooperation\\Cello Manager");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool IsValidFilename(ref string testName)
        {

            if (_containsABadCharacter.IsMatch(testName)) { return false; }

            try
            {
                testName = Path.GetFullPath(testName);
                var arr = DriveInfo.GetDrives().Select(d => d.Name).ToArray();

                string name = testName;
                if (!arr.Any(s => name.StartsWith(s))) return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}