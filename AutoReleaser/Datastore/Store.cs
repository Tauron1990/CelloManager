using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FileMode = System.IO.FileMode;

namespace AutoReleaser.Datastore
{
    public sealed class Store
    {
        [Serializable]
        private class Container
        {
            public List<ReleaseItem> ReleaseItems { get; } = new List<ReleaseItem>();
            public Options Options { get; } = new Options();
        }

        private const string FileName = "App.db";

        private static Store _store;
        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        private readonly Container _container;
        
        private Store()
        {
            try
            {
                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
                if(File.Exists(file))
                    using (var stream = new FileStream(file, FileMode.Open))
                        _container = (Container) BinaryFormatter.Deserialize(stream);
                else
                    _container = new Container();
            }
            catch (SerializationException)
            {
                _container = new Container();
            }
        }

        public static Store StoreInstance => _store ?? (_store = new Store());

        public Options GetOptions()
        {
            return _container.Options;
        }

        public void PushReleaseItem(ReleaseItem item)
        {
            _container.ReleaseItems.Add(item);
            SaveContainer();
        }

        public void SaveContainer()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            using (var stream = new FileStream(file, FileMode.Create))
                BinaryFormatter.Serialize(stream, _container);
        }

        public IEnumerable<ReleaseItem> GetReleaseItems()
        {
            return _container.ReleaseItems;
        }
    }
}