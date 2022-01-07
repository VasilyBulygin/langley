using langley.core;
using ReactiveUI;

namespace langley.gui.ViewModels
{
    public class ConnectionSettingsViewModel : BaseViewModel<ConnectionSettings>
    {
        public ConnectionSettingsViewModel(ConnectionSettings model) : base(model)
        {
        }

        public int Port
        {
            get => Model.Port;
            set
            {
                Model.Port = value;
                this.RaisePropertyChanged();
            }
        }

        public string Address
        {
            get => Model.Address;
            set
            {
                Model.Address = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
