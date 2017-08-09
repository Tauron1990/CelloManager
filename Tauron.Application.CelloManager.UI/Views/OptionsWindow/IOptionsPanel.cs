using System.ComponentModel;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow
{
    public interface IOptionsPanel : INotifyPropertyChanged
    {
        bool HasErrors { get; }

        void Reset();

        void Commit();

        void Rollback();
    }
}