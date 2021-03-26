using System;
using System.Collections.Generic;
using Tauron.Application.CelloManager.Logic.RefillPrinter;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IManagerEnviroment))]
    public sealed class ManagerEnviroment :  IManagerEnviroment, INotifyBuildCompled
    {
        [Serializable]
        private class SettingImpl : ISettings
        {
            private readonly Dictionary<string, string> _cache;

            public SettingImpl(Dictionary<string, string> cache)
            {
                _cache = cache;
                MaximumSpoolHistorie = 256;
            }

            public string Dns
            {
                get => GetValue("DNS", "8.8.8.8");
                set => _cache["DNS"] = value;
            }

            public string TargetEmail
            {
                get => GetValue("TargetEmail");
                set => _cache["TargetEmail"] = value;
            }

            public RefillPrinterType PrinterType
            {
                get => GetValue("PrinterType").TryParseEnum(RefillPrinterType.Print);
                set => _cache["PrinterType"] = value.ToString();
            }

            public bool Purge
            {
                get => bool.TryParse(GetValue("Purge"), out var result) && result;
                set => _cache["Purge"] = value.ToString();
            }

            public int Threshold 
            {
                get => int.TryParse(GetValue("Threshold"), out var valueResult) ? valueResult : 2;
                set => _cache["Threshold"] = value.ToString();
            }

            public string DefaultPrinter
            {
                get => GetValue("DefaultPrinter");
                set => _cache["DefaultPrinter"] = value;
            }

            public int MaximumSpoolHistorie
            {
                get => int.TryParse(GetValue("MaximumSpoolHistorie"), out var valueResult) ? valueResult : -1;
                set => _cache["MaximumSpoolHistorie"] = value.ToString();
            }
            public string DockingState
            {
                get => GetValue("DockingState");
                set => _cache["DockingState"] = value;
            }

            public string SpoolDataGridState
            {
                get => GetValue("SpoolDataGridState");
                set => _cache["SpoolDataGridState"] = value;
            }

            public long EmailPort
            {
                get => long.TryParse(GetValue("EmailPort", "25"), out var valueResult) ? valueResult : 25;
                set => _cache["EmailPort"] = value.ToString();
            }

            public string UserName
            {
                get => GetValue("UserName");
                set => _cache["UserName"] = value;
            }

            public string Password
            {
                get => GetValue("Password");
                set => _cache["Password"] = value;
            }

            public string Server
            {
                get => GetValue("Server");
                set => _cache["Server"] = value;
            }

            public bool DomainMode
            {
                get => bool.TryParse("DomainMode", out var vaResult) ? vaResult : false;
                set => _cache["DomainMode"] = value.ToString();
            }

            public string Domain
            {
                get => GetValue("Domain");
                set => _cache["Domain"] = value;
            }

            private string GetValue(string key, string defaultValue = null)
            {
                return _cache.TryGetValue(key, out var value) ? value : defaultValue ?? String.Empty;
            }
        }

        [Inject]
        public IOptionsRepository OptionsRepository { private get; set; }

        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public ManagerEnviroment()
        {
            GetDicPath().CreateDirectoryIfNotExis();
            Settings = new SettingImpl(_cache);
        }

        public ISettings Settings { get; }

        public void Save()
        {
            OptionsRepository.Save(_cache);
        }

        public void Reload()
        {
            OptionsRepository.Fill(_cache);
        }

        private static string GetDicPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager");

        public void BuildCompled() => Reload();
    }
}