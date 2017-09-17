using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IManagerEnviroment))]
    public sealed class ManagerEnviroment : RepositoryBase, IManagerEnviroment, INotifyBuildCompled
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

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

        public ISettings Settings { get; }

        public void Save()
        {
            using (Manager.StartOperation())
            {
                List<OptionEntry> entrys = CoreManager.Database.OptionEntries.ToList();

                var keyValuePairs = new Dictionary<string, string>(_cache);

                foreach (var optionEntry in entrys)
                {
                    if (!keyValuePairs.TryGetValue(optionEntry.key, out var usedValue)) continue;

                    optionEntry.Value = usedValue;
                    keyValuePairs.Remove(optionEntry.key);
                }

                foreach (var pair in keyValuePairs)
                {
                    CoreManager.Database.OptionEntries.Add(new OptionEntry {key = pair.Key, Value = pair.Value});
                }
            }
        }

        private static string GetDicPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager");
        }

        public void BuildCompled()
        {
            using (Manager.StartOperation())
            {
                foreach (var optionEntry in CoreManager.Database.OptionEntries)
                    _cache[optionEntry.key] = optionEntry.Value;
            }
        }
    }
}