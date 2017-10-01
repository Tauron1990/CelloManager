using Tauron.Application.CelloManager.Data.Core;

namespace TestHelpers.Mocks
{
    public class ManagerEnviromentMock : IManagerEnviroment
    {
        public ManagerEnviromentMock()
        {
            Settings = new SettingsMock
            {
                MaximumSpoolHistorie = 5
            };
        }

        public ISettings Settings { get; set; }

        public void Save()
        {
        }
    }
}