using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LiteDB;
using FileMode = System.IO.FileMode;

namespace AutoReleaser.Datastore
{
    public sealed class Store
    {
        private const string FileName = "App.db";
        private const string OptionsFileName = "Options.bin";

        private static Store _store;
        private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

        private readonly LiteDatabase _database;

        private Store()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            _database = new LiteDatabase(file);
        }

        public static Store StoreInstance => _store ?? (_store = new Store());

        public Options GetOptions()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OptionsFileName);

            if (!File.Exists(file)) return new Options();

            using (var stream = new FileStream(file, FileMode.Open))
                return (Options)_binaryFormatter.Deserialize(stream);
        }

        public void UpdateOptions(Options options)
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OptionsFileName);
            using (var stream = new FileStream(file, FileMode.OpenOrCreate))
                _binaryFormatter.Serialize(stream, options);
        }

        public void PushReleaseItem(ReleaseItem item)
        {
            _database.GetCollection<ReleaseItem>().Insert(item);
        }

        public void UpdateReleaseItem(ReleaseItem item)
        {
            _database.GetCollection<ReleaseItem>().Update(item);
        }

        public IEnumerable<ReleaseItem> GetReleaseItems()
        {
            var coll = _database.GetCollection<ReleaseItem>();
            coll.EnsureIndex(i => i.InitialTime);

            return coll.Find(Query.All(nameof(ReleaseItem.InitialTime), Query.Descending), 0, 10);
        }
    }
}