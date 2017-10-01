using System.Collections.Generic;

namespace Tauron.Application.CelloManager.Data.Core
{
    public interface IOptionsRepository
    {
        void Fill(Dictionary<string, string> options);

        void Save(Dictionary<string, string> options);
    }
}