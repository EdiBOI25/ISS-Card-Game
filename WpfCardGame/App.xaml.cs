using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using WpfCardGame.network;

namespace WpfCardGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SteamManager _steamManager = new SteamManager();

        void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Ask the user if they want to allow the session to end
            string msg = string.Format("{0}. End session?", e.ReasonSessionEnding);
            MessageBoxResult result = MessageBox.Show(msg, "Session Ending", MessageBoxButton.YesNo);

            // End session, if specified
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;

            }
            _steamManager.Shutdown();
            Trace.WriteLine("[App.cs] Closed via App_SessionEnding");
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                _steamManager.Initialize();
            }
            catch
            {
                this.Shutdown();
            }
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _steamManager.Shutdown();
            Trace.WriteLine("[App.cs] Closed via App_OnExit");
        }

        private void OnMainWindowClose(object sender, EventArgs e)
        {
            _steamManager.Shutdown();
            Trace.WriteLine("[App.cs] Closed via OnMainWindowClose");
        }

    }
}
