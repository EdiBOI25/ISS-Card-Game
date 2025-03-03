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
using Steamworks;
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
        private SteamLobby steamLobby;

        public MainWindow()
        {
            InitializeComponent();
            steamLobby = new SteamLobby();
            Player ediboi = Player.CreateFromSteam();
            Player player2 = Player.CreateFromSteamId(new CSteamID(76561198907576762));


            //Image myImage = new Image { Width = 184 };

            BitmapSource imageSource = ediboi.Avatar;
            Player1Avatar.Source = imageSource;
            Player1Avatar.UpdateLayout();
            Player1Name.Text = "Player name from Steam: " + ediboi.Name + "\nID: " + ediboi.SteamId;

            BitmapSource imageSource2 = player2.Avatar;
            Player2Avatar.Source = imageSource2;
            Player2Avatar.UpdateLayout();
            Player2Name.Text = "Player name from Steam: " + player2.Name + "\nID: " + player2.SteamId;

            SetCompatibilityRating();
        }

        private async void SetCompatibilityRating()
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < 30; ++i)
                {
                    int randomUshort = random.Next(0, 100);
                    await CompatibilityRating.Dispatcher.InvokeAsync(() =>
                    {
                        CompatibilityRating.Text = "Compatibility: " + randomUshort + "%";
                    });
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
                CompatibilityRating.Text = "Compatibility: " + 69 + "%";
            }
        }

        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            steamLobby.CreateLobby();
        }

        private void FindLobby_Click(object sender, RoutedEventArgs e)
        {
            steamLobby.FindLobby();
        }
    }
}