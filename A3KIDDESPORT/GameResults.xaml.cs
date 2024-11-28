using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataManagement;
using DataManagement.Models;
using Dapper;
using System.Data.SqlClient;
using System.Text.RegularExpressions;


namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for GameResults.xaml
    /// </summary>
    public partial class GameResults : Window
    {
        DataAdapter data = new DataAdapter();
        List<ResultView> teamResultView;
        List<TeamDetail> teamList;

        bool isNewEntry = true;

        public GameResults()
        {
            InitializeComponent();
            SetupComboBox();

        }


        private void UpdateDataGrid()
        {
            teamResultView = data.GetAllTeamResults();

        }
        private void SetupComboBox()
        {
            teamList = data.GetAllTeamDetails();
            cboTeamName.ItemsSource = teamList;
            cboTeamName.DisplayMemberPath = "TeamName";
            cboTeamName.SelectedValuePath = "TeamID";

            teamList = data.GetAllTeamDetails();
            cboOpponentName.ItemsSource = teamList;
            cboOpponentName.DisplayMemberPath = "TeamName";
            cboOpponentName.SelectedValuePath = "TeamID";



        }






        private bool IsFormFilledCorrectly()
        {
            if (cboTeamName.SelectedIndex == -1)
            {
                return false;
            }
            if (cboOpponentName.SelectedIndex == -1)
            {
                return false;
            }
            return true;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("Please ensure form is filled correctyl before saving!\n" +
                               "-Ensure all combo box you chosed.\n");

                return;
            }
        
            TeamDetail selectedTeam = cboTeamName.SelectedItem as TeamDetail;
            TeamDetail selectedOpponent = cboOpponentName.SelectedItem as TeamDetail;

            if (selectedTeam == null && selectedOpponent == null)
            {
                MessageBox.Show("Select both teams in boxes");
                return;
            }
            if (rbnWinner.IsChecked == true)
            {
                selectedTeam.CompetitionPoints = 2;
                data.PerformTransaction(selectedTeam, selectedTeam.TeamID);
            }
            else if (rbnOpponentWinner.IsChecked == true)
            {
                selectedOpponent.CompetitionPoints = 2;
                data.PerformTransaction(selectedOpponent, selectedOpponent.TeamID);
            }
            else if (rbnDraw.IsChecked == true)
            {
                selectedTeam.CompetitionPoints = 1;
                selectedOpponent.CompetitionPoints = 1;
                data.PerformTransaction(selectedTeam, selectedTeam.TeamID);
                data.PerformTransaction(selectedOpponent, selectedOpponent.TeamID);
            }


            UpdateDataGrid();

            MessageBox.Show("Saving is success!!");

            

        }

        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedTeamID = cboTeamName.SelectedIndex;
            int selectedOpponentID = cboOpponentName.SelectedIndex;

            if (selectedTeamID == selectedOpponentID)
            {
                MessageBox.Show("Can't selected the same Team");
                cboTeamName.SelectedIndex = -1;
            }
        }
    }
}
