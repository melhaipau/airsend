
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AirSend.ViewModels
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
