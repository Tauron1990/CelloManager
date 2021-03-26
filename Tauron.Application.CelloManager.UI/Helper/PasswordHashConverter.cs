using System.Windows.Data;
using Tauron.Application.CelloManager.Logic;
using Tauron.Application.Converter;

namespace Tauron.Application.CelloManager.UI.Helper
{
    public class PasswordHashConverter : ValueConverterFactoryBase
    {
        private class Converter : StringConverterBase<string>
        {
            private static IPasswordHasher _passwordHasher =
                CommonApplication.Current.Container.Resolve<IPasswordHasher>();
            
            protected override bool CanConvertBack { get; } = true;

            protected override string Convert(string value) => _passwordHasher.GetPassword(value);

            protected override string ConvertBack(string value) => _passwordHasher.HashPassword(value);
        }

        protected override IValueConverter Create() => new Converter();

    }
}