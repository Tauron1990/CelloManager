using System.Windows.Data;
using Tauron.Application.CelloManager.Logic.Historie;
using Tauron.Application.CelloManager.Resources;
using Tauron.Application.Converter;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public class CommittedRefillFormatter : ValueConverterFactoryBase
    {
        private class ConverterImpl : StringConverterBase<CommittedRefill>
        {
            protected override string Convert(CommittedRefill value)
            {
                return
                    $"{value.Id} - {UIResources.OrderViewRefillSendLabel} {value.SentTime} - {UIResources.OrderViewRefillCompledLabel} {value.CompledTime} - {UIResources.OderViewOrderedCount} {value.Count}";
            }
        }

        private static readonly IValueConverter ValueConverter = new ConverterImpl();

        protected override IValueConverter Create() => ValueConverter;
    }
}