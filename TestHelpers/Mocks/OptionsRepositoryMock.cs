using System.Collections.Generic;
using Tauron.Application.CelloManager.Data.Core;

namespace TestHelpers.Mocks
{
    public class OptionsRepositoryMock : IOptionsRepository
    {
        public void Fill(Dictionary<string, string> options)
        {
            options["MaximumSpoolHistorie"] = "5";
        }

        public void Save(Dictionary<string, string> options)
        {
            
        }
    }
}