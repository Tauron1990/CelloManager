using System.Windows.Data;
using System.Windows.Media;
using Tauron.Application.Converter;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public sealed class BorderEditConverter : ValueConverterFactoryBase
    {
        private class Converter : ValueConverterBase<bool, Brush>
        {
            protected override Brush Convert(bool value) => value ? Brushes.LawnGreen : Brushes.Red;
        }

        protected override IValueConverter Create() => new Converter();
    }
}