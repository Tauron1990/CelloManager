using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Models
{
    [ExportModel(UIModule.OperationContextModelName)]
    public sealed class OperationContextModel : ModelBase
    {
        private bool _isOperationRunning;

        public bool IsOperationRunning
        {
            get => _isOperationRunning;
            set { _isOperationRunning = value; OnPropertyChanged();}
        }
    }
}