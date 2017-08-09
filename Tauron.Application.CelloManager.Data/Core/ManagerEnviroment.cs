using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IManagerEnvioment))]
    public sealed class ManagerEnviroment : IManagerEnvioment
    {
        [Serializable]
        private class SerializerContainer
        {
            //public LayoutImpl CelloRepository;
            public SettingImpl Setting;
        }

        //[Serializable]
        //private class LayoutImpl : ICelloRepository
        //{
        //    [Serializable]
        //    private class CelloSpool : ObservableObject, ICelloSpool
        //    {
        //        private string _name;
        //        private int _amount;
        //        private int _neededamount;
        //        private string _category;

        //        public string Name
        //        {
        //            get { return _name; }
        //            set { _name = value; OnPropertyChanged();}
        //        }

        //        public string Type
        //        {
        //            get { return _category; }
        //            set { _category = value; OnPropertyChanged();}
        //        }

        //        public int Amount
        //        {
        //            get { return _amount; }
        //            set { _amount = value; OnPropertyChanged();}
        //        }

        //        public int Neededamount
        //        {
        //            get { return _neededamount; }
        //            set { _neededamount = value; OnPropertyChanged();}
        //        }
        //    }

        //    private readonly Dictionary<string, ICelloSpool> _entries;
        //    [NonSerialized]
        //    private IEventAggregator _aggregator;

        //    public IEnumerable<ICelloSpool> SpoolEntries => _entries.Values;
        //    public IEventAggregator Aggregator
        //    {
        //        set { _aggregator = value; }
        //    }

        //    public LayoutImpl(IEventAggregator aggregator)
        //    {
        //        Aggregator = aggregator;
        //        _entries = new Dictionary<string, ICelloSpool>();
        //    }

        //    public bool Add(string name, string type, int amount, int neededamount)
        //    {
        //        if (_entries.ContainsKey(name)) return false;

        //        var temp = new CelloSpool { Amount = amount, Name = name, Neededamount = neededamount, Type = type };
        //        _entries[name] = temp; 

        //        _aggregator.GetEvent<CelloSpoolEntryAddEvent, ICelloSpool>().Publish(temp);

        //        return true;
        //    }

        //    public void Remove(string name)
        //    {
        //        _entries.Remove(name);
        //    }
        //}

        [Serializable]
        private class SettingImpl : ISettings
        {
            public SettingImpl()
            {
                MaximumSpoolHistorie = 256;
            }

            public string DefaultPrinter { get; set; }
            public int MaximumSpoolHistorie { get; set; }
        }

        public ManagerEnviroment()
        {
            var filePath = GetSettingsFile();
            if (filePath.CreateDirectoryIfNotExis())
            {
                //CelloRepository = new LayoutImpl(EventAggregator.Aggregator);
                Settings = new SettingImpl();
                return;
            }

            if (!filePath.ExisFile())
            {
                //CelloRepository = new LayoutImpl(EventAggregator.Aggregator);
                Settings = new SettingImpl();
                return;
            }

            try
            {
                using (var stream = GetSettingsFile().OpenRead())
                {
                    var container = new BinaryFormatter().Deserialize(stream) as SerializerContainer;
                    Settings = container == null ? new SettingImpl() : container.Setting;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is SerializationException)
            {
                LogManager.GetLogger(AppConststands.ManagerEnviromentPolicy, typeof(ManagerEnviroment)).Error(ex);

                //CelloRepository = new LayoutImpl(EventAggregator.Aggregator);
                Settings = new SettingImpl();
            }
        }

        public ISettings Settings { get; }

        public void Save()
        {
            using (var stream = GetSettingsFile().OpenWrite())
            {
                new BinaryFormatter().Serialize(stream, new SerializerContainer {Setting = (SettingImpl) Settings});
            }
            //new SerializerContainer {CelloRepository = (LayoutImpl) CelloRepository, Setting = (SettingImpl) Settings});
        }

        private static string GetDicPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).CombinePath("Tauron\\CelloManager");
//            return
//                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
//                    .CombinePath("Tauron\\CelloManager");
        }

        private static string GetSettingsFile()
        {
            return GetDicPath().CombinePath("Settings.dat");
        }
    }
}