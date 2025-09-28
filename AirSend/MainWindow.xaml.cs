
using System.Windows;
using AirSend.ViewModels;

namespace AirSend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
