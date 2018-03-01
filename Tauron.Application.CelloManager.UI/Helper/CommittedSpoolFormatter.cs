using System.Windows.Data;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.Converter;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public class CommittedSpoolFormatter : ValueConverterFactoryBase
    {
        private class ConverterImpl : StringConverterBase<CommittedSpool>
        {
            protected override string Convert(CommittedSpool value)
            {
                return $"{value.SpoolId} - {value.Name} {value.Type} - {UIResources.OderViewOrderedCount} {value.OrderedCount}";
            }
        }

        private static readonly IValueConverter ValueConverter = new ConverterImpl();

        protected override IValueConverter Create() => ValueConverter;
    }
}