using System;
using System.ComponentModel;

namespace Tauron.Application.CelloManager.UI.Views.OptionsWindow
{
    public interface IOptionsPanel : INotifyPropertyChanged
    {
        event EventHandler LockUIEvent;
        //event EventHandler UnLockUIEvent;

        bool HasErrors { get; }

        void Reset();

        void Commit();

        void Rollback();
    }
}