using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IOptionsRepository))]
    public class OptionsRepository : IOptionsRepository
    {
        private readonly CoreDatabase _database;

        [Inject]
        public OptionsRepository(CoreDatabase database)
        {
            _database = database;
        }

        public void Fill(Dictionary<string, string> options)
        {
            foreach (var optionEntry in _database.OptionEntries)
                options[optionEntry.key] = optionEntry.Value;
        }

        public void Save(Dictionary<string, string> options)
        {
            List<OptionEntry> entrys = _database.OptionEntries.ToList();

            var keyValuePairs = new Dictionary<string, string>(options);

            foreach (var optionEntry in entrys)
            {
                if (!keyValuePairs.TryGetValue(optionEntry.key, out var usedValue)) continue;

                optionEntry.Value = usedValue;
                keyValuePairs.Remove(optionEntry.key);
            }

            foreach (var pair in keyValuePairs)
            {
                _database.OptionEntries.Add(new OptionEntry { key = pair.Key, Value = pair.Value });
            }
        }
    }
}