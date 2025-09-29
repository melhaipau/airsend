using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace AirSend.Utils
{
    public static class Log
    {
        public static ObservableCollection<string> Lines { get; } = new();

        public static void Info(string message) => Write("INFO", message);
        public static void Warn(string message) => Write("WARN", message);
        public static void Error(string message) => Write("ERROR", message);

        private static void Write(string level, string message)
        {
            var line = $"[{DateTime.Now:HH:mm:ss}] {level}: {message}";
            try
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Append(line)));
                }
                else
                {
                    Append(line);
                }
            }
            catch
            {
                // As a last resort, try direct append (non-UI contexts)
                Append(line);
            }
            System.Diagnostics.Debug.WriteLine(line);
        }

        private static void Append(string line)
        {
            Lines.Add(line);
            if (Lines.Count > 1000) Lines.RemoveAt(0);
        }
    }
}
