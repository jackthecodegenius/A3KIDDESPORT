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

namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            conMain.Content = new TeamDetailPanel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Do you want to save changes?";
            string caption = "Event";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }

            else if (result == MessageBoxResult.No)
            {
                Close();
            }
        }

        private void btnTeamDetails_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new TeamDetailPanel();
        }

        private void btnEvents_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new EventPanel();
        }

        private void btnGamesPlayed_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new GamePanel();
        }

        private void btnTeamResults_Click(object sender, RoutedEventArgs e)
        {
            conMain.Content = new TeamResultPanel();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            ReportPage ReportPageWindow = new ReportPage();

            ReportPageWindow.Show();
        }
    }
}