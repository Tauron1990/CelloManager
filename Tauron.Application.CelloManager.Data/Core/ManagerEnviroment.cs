using System;
using System.Collections.Generic;
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

            private string GetValue(string key)
            {
                return _cache.TryGetValue(key, out var value) ? value : String.Empty;
            }
        }

        public ManagerEnviroment()
        {
            GetDicPath().CreateDirectoryIfNotExis();
            Settings = new SettingImpl(_cache);
        }

        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();


        public ISettings Settings { get; }

        [Inject]
        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public void Save()
        {
            using (var work = UnitOfWorkFactory.CreateUnitOfWork())
                work.OptionsRepository.Save(_cache);
        }

        private static string GetDicPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager");
        }

        public void BuildCompled()
        {
            using (var work = UnitOfWorkFactory.CreateUnitOfWork())
                work.OptionsRepository.Fill(_cache);
        }
    }
}