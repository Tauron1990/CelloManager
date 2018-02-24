using Tauron.Application.CelloManager.Logic.Manager;
using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Models
{
    public class EditSpool : ObservableObject
    {
        private bool _isNew;
        private bool _isDeleted;

        public CelloSpool Spool { get; }

        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        public bool IsDeleted
        {
            get => _isDeleted;
            set => SetProperty(ref _isDeleted, value);
        }

        public ModelBase UIViewSpool { get; set; }

        public bool IsOk => UIViewSpool?.HasNoErrors ?? false;

        public EditSpool(CelloSpool spool)
        {
            Spool = spool;
        }
    }
}