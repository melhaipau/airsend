using System.Windows;
using AirSend.ViewModels;
using AirSend.Views;   // for DebugWindow

namespace AirSend
{
    public partial class MainWindow : Window
    {
        // exactly one field
        private DebugWindow? _debugWindow;

        // exactly one constructor
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        // exactly one handler with this name
        private void OpenDebug_Click(object sender, RoutedEventArgs e)
        {
            if (_debugWindow == null || !_debugWindow.IsVisible)
            {
                _debugWindow = new DebugWindow { Owner = this };
                _debugWindow.Show();
            }
            else
            {
                _debugWindow.Activate();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "AirSend — demo sender for AirPlay discovery & audio capture.\n\nNote: Replace MockRaopSender with a real RAOP/AirPlay sender to stream audio.",
                "About AirSend",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
