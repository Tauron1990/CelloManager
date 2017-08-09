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

        public static string BuildUinqueId(CelloSpoolBase sbase)
        {
            return BuildUinqueId(sbase.Name, sbase.Type);
        }

        public static string BuildUinqueId(string name, string type)
        {
            return name + "+" + type;
        }

        public abstract void UpdateSpool();
    }
}