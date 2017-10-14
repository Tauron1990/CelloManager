using Tauron.Application.CelloManager.Data.Core;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.Logic.Manager
{
    public abstract class CelloSpoolBase : ModelBase
    {
        public abstract string UniquieId { get; }

        public abstract string Name { get; set; }

        public abstract string Type { get; set; }

        public abstract int Amount { get; set; }

        public abstract int Neededamount { get; set; }

        public abstract int Id { get; }
    }
}