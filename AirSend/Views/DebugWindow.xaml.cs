using System.Linq;
using System.Text;
using System.Windows;
using AirSend.Utils;

namespace AirSend.Views
{
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();

            // Bind in code for robustness
            LogList.ItemsSource = Log.Lines;

            // Scroll to last item when loaded
            Loaded += (_, __) => SafeScrollToEnd();

            // Keep following new lines
            Log.Lines.CollectionChanged += (_, __) => SafeScrollToEnd();
        }

        private void SafeScrollToEnd()
        {
            if (LogList.Items.Count == 0) return;
            var last = LogList.Items[LogList.Items.Count - 1];
            LogList.ScrollIntoView(last);
        }

        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            var text = string.Join("\n", Log.Lines.ToArray());
            Clipboard.SetText(text);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Log.Lines.Clear();
        }
    }
}
