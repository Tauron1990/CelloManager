using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;

namespace Tauron.Application.CelloManager.Data.Core
{
    [Export(typeof(IOptionsRepository))]
    public class OptionsRepository : IOptionsRepository
    {
        public void Fill(Dictionary<string, string> options)
        {
            using (var database = new CoreDatabase())
                foreach (var optionEntry in database.OptionEntries)
                    options[optionEntry.key] = optionEntry.Value;
        }

        public void Save(Dictionary<string, string> options)
        {

            using (var database = new CoreDatabase())
            {
                List<OptionEntity> entrys = database.OptionEntries.ToList();

                var keyValuePairs = new Dictionary<string, string>(options);

                foreach (var optionEntry in entrys)
                {
                    if (!keyValuePairs.TryGetValue(optionEntry.key, out var usedValue)) continue;

                    optionEntry.Value = usedValue;
                    keyValuePairs.Remove(optionEntry.key);
                }

                foreach (var pair in keyValuePairs)
                {
                    database.OptionEntries.Add(new OptionEntity { key = pair.Key, Value = pair.Value });
                }

                database.SaveChanges();
            }
        }
    }
}