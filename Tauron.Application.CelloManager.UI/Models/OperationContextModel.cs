using Tauron.Application.Models;

namespace Tauron.Application.CelloManager.UI.Models
{
    [ExportModel(UIModule.OperationContextModelName)]
    public sealed class OperationContextModel : ModelBase
    {
        private bool _isEditingOperationRunning;
        private bool _isOperationRunning;

        public bool IsEditingOperationRunning
        {
            get => _isEditingOperationRunning;
            set => SetProperty(ref _isEditingOperationRunning, value);
        }

        public bool IsOperationRunning
        {
            get => _isOperationRunning;
            set => SetProperty(ref _isOperationRunning, value);
        }
    }
}