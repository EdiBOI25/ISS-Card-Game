using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Steamworks;
using WpfCardGame.controller;
using WpfCardGame.domain;
using WpfCardGame.network;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace WpfCardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SteamLobby _steamLobby;
        private DispatcherTimer _steamUpdateTimer = new DispatcherTimer();
        private bool _toggleOpacity = false;


        public MainWindow()
        {
            InitializeComponent();
            KeepSteamRunning();
            _steamLobby = new SteamLobby();

            Player ediboi = Player.CreateFromSteam();
            BitmapSource imageSource = ediboi.Avatar;
            Player1Avatar.Source = imageSource;
            Player1Avatar.UpdateLayout();
            Player1Name.Text = "Player name from Steam: " + ediboi.Name + "\nID: " + ediboi.SteamId;

            _steamLobby.FindLobby();
        }

        private void KeepSteamRunning()
        {
            _steamUpdateTimer.Interval = TimeSpan.FromMilliseconds(33);
            _steamUpdateTimer.Tick += (sender, e) => SteamAPI.RunCallbacks();
            _steamUpdateTimer.Tick += ForceRefresh;
            _steamUpdateTimer.Start();
        }

        private void ForceRefresh(object? sender, EventArgs e)
        {
            _toggleOpacity = !_toggleOpacity;
            RefreshOverlay.Opacity = _toggleOpacity ? 0.99 : 1.0; // Tiny change forces redraw
        }

        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            _steamLobby.CreateLobby();
        }

        private void RefreshLobbies()
        {
            var viewModel = (LobbyViewModel)this.DataContext;
            List<Lobby> lobbies = _steamLobby.Lobbies;
            viewModel.UpdateLobbies(lobbies);
        }

        private void FindLobby_Click(object sender, RoutedEventArgs e)
        {
            _steamLobby.FindLobby();
        }

        protected override void OnClosed(EventArgs e)
        {
            _steamUpdateTimer.Stop();
            base.OnClosed(e);
        }
    }
}