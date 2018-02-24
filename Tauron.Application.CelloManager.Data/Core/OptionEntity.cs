using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.CelloManager.Data.Core
{
    public class OptionEntity : GenericBaseEntity<int>
    {
        private string _key;
        private string _value;

        public string key
        {
            get => _key;
            set => SetWithNotify(ref _key, value);
        }

        public string Value
        {
            get => _value;
            set => SetWithNotify(ref _value, value);
        }
    }
}