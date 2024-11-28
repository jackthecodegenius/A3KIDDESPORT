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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataManagement;
using DataManagement.Models;
using Dapper;
using System.Data.SqlClient;

namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for TeamResultPanel.xaml
    /// </summary>
    public partial class TeamResultPanel : UserControl
    {
        //Class used to manage our database interactions.
        DataAdapter data = new DataAdapter();
        //Lists to hold the combo box details.
        List<Event> eventList;
        List<Game> gameList;
        //List to hold the data grid contents
        List<TeamDetail> teamList;
        List<ResultView> resultViewList;
        //Acts as a switch/flag to determine whether we are saving a new entry or updating
        //an entry when the save button is pressed.
        bool isNewEntry = true;

        public TeamResultPanel()
        {
            InitializeComponent();
            SetupComboBoxes();
            UpdateDataGrid(); 
        }

        private void UpdateDataGrid()
        {
            //Retrieves the table data from the database and assigns it to the list.
            resultViewList = data.GetAllTeamResults();
            //Sets the list as the data source for the data grid.
            dgvTeamResult.ItemsSource = resultViewList;
            //Refreshes the data grid diplay to match the current list contents.
            dgvTeamResult.Items.Refresh();
        }

        private void SetupComboBoxes()
        {
            //Retrieves the table data from the database and assigns it to the list.
            eventList = data.GetAllEvents();
            //Sets the list as the data source for the combo box.
            cboEvent.ItemsSource = eventList;
            //Sets the display of the combo box to show the details of each entry that are in the
            //chosen field. In this case, the 'Name' property.
            cboEvent.DisplayMemberPath = "EventName";
            //Sets the value associated with the combo box to be the details in the chosen field
            //of the list models. In this case, the 'Id' property(primary keys). This is the value
            // thet is actually given once a seleciton is made in the combo box.
            cboEvent.SelectedValuePath = "EventID";

            //Repeats the above steps for the other combo box.
            gameList = data.GetAllGames();
            cboGame.ItemsSource = gameList;
            cboGame.DisplayMemberPath = "GameName";
            cboGame.SelectedValuePath = "GameID";

            teamList = data.GetAllTeamDetails();
            cboTeam.ItemsSource = teamList;
            cboTeam.DisplayMemberPath = "TeamName";
            cboTeam.SelectedValuePath = "TeamID";

            teamList = data.GetAllTeamDetails();
            cboOppositeTeam.ItemsSource = teamList;
            cboOppositeTeam.DisplayMemberPath = "TeamName";
            cboOppositeTeam.SelectedValuePath = "TeamID";



        }

        private void ClearEntryFormFields()
        {
            txtTeamResultID.Text = string.Empty;
            cboEvent.SelectedIndex = -1;
            cboGame.SelectedIndex = -1;
            cboTeam.SelectedIndex = -1;
            cboOppositeTeam.SelectedIndex = -1;
            cboResult.SelectedIndex = -1;
            //Set the save mode to new
            isNewEntry = true;
        }

        private bool IsFormFilledCorrectly()
        {
            if (cboEvent.SelectedIndex == -1)
            {
                return false;
            }
            if (cboGame.SelectedIndex == -1)
            {
                return false;
            }
            if (cboTeam.SelectedIndex == -1)
            {
                return false;
            }
            if (cboOppositeTeam.SelectedIndex == -1)
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(cboResult.Text))
            {
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Check if the user has filled the data entry fields properly, otherwise
            //pop up a message box to infrom them there is an error.
            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("Please ensure form is filled correctyl before saving!\n" +
                                "-Ensure all sections are filled/selected\n" );
                return;
            }
            //Create a new expense model object
            TeamResult currentteamResult = new TeamResult();
            //Retrieve the form details and put them into the model.
            currentteamResult.EventID = (int)cboEvent.SelectedValue;
            currentteamResult.GameID = (int)cboGame.SelectedValue;
            currentteamResult.TeamID = (int)cboTeam.SelectedValue;
            currentteamResult.OppositeTeamID = (int)cboOppositeTeam.SelectedValue;
            currentteamResult.Result = cboResult.Text;

            if (isNewEntry)
            {
                //Hand the expense model over to be saved as a new entry to the database.
                data.AddNewTeamResult(currentteamResult);
            }
            else
            {
                //Get the Id from the text fields of the record being updated.
                currentteamResult.TeamResultID = int.Parse(txtTeamResultID.Text);
                //Hand the expense model over to update the associated record.
                data.UpdateTeamResult(currentteamResult);
            }

            TeamDetail selectedTeam = cboTeam.SelectedItem as TeamDetail;
            TeamDetail selectedOpponent = cboOppositeTeam.SelectedItem as TeamDetail;

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

            //Refresh the data grid and clear the form fields.
            UpdateDataGrid();
            ClearEntryFormFields();
        }



        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearEntryFormFields();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Make sure the table has a valid row currently selected
            if (dgvTeamResult.SelectedIndex < 0)
            {
                return;
            }
            //Pop uip a message box to confrim the deletion by the user.
            MessageBoxResult result = MessageBox.Show($"Delete '{resultViewList[dgvTeamResult.SelectedIndex].Result}' team result?",
                                                      "Delete Confirmation",
                                                      MessageBoxButton.YesNo);
            //If the user did confirm by pressing yes.
            if (result == MessageBoxResult.Yes)
            {
                // Grab the Id of the selected record
                int id = resultViewList[dgvTeamResult.SelectedIndex].ResultViewID;
                // Retrieve the record before deletion to get its details
                TeamResult deletedTeamResult = data.GetTeamResultByTeamResultID(id);
                // Delete the record
                data.DeleteTeamResult(id);

                if (deletedTeamResult != null)
                {
                    if (deletedTeamResult.Result == "Win")
                    {
                        // Decrement competition points for the winning team
                        TeamDetail winningTeam = teamList.FirstOrDefault(t => t.TeamID == deletedTeamResult.TeamID);
                        if (winningTeam != null)
                        {
                            winningTeam.CompetitionPoints = - 2;
                            data.PerformTransaction(winningTeam, winningTeam.TeamID);
                        }
                    }
                    else if (deletedTeamResult.Result == "Opponent Win")
                    {
                        // Decrement competition points for the opponent team
                        TeamDetail opponentTeam = teamList.FirstOrDefault(t => t.TeamID == deletedTeamResult.OppositeTeamID);
                        if (opponentTeam != null)
                        {
                            opponentTeam.CompetitionPoints = - 2;
                            data.PerformTransaction(opponentTeam, opponentTeam.TeamID);
                        }
                    }
                    else if (deletedTeamResult.Result == "Draw")
                    {
                        // Decrement competition points for both teams
                        TeamDetail team1 = teamList.FirstOrDefault(t => t.TeamID == deletedTeamResult.TeamID);
                        TeamDetail team2 = teamList.FirstOrDefault(t => t.TeamID == deletedTeamResult.OppositeTeamID);
                        if (team1 != null && team2 != null)
                        {
                            team1.CompetitionPoints = - 1;
                            team2.CompetitionPoints = - 1;
                            data.PerformTransaction(team1, team1.TeamID);
                            data.PerformTransaction(team2, team2.TeamID);
                        }
                    }
                }

                // Confirm deletion to user
                MessageBox.Show("Record Deleted.");
                // Refresh the data grid and clear the form fields
                UpdateDataGrid();
                ClearEntryFormFields();
            }
        }
        

        private void dgvTeamResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Make sure the table has a valid row currently selected
            if (dgvTeamResult.SelectedIndex < 0)
            {
                return;
            }
            //Grab the Id(primary key) from the entry in the list that matcheds the row
            //selected in the table.
            int id = resultViewList[dgvTeamResult.SelectedIndex].ResultViewID;
            //Retreieve the expense record from the database that matches the selected id.
            TeamResult selectedTeamResult = data.GetTeamResultByTeamResultID(id);
            //Set the text fields to match the values in the model for their associated properties.
            txtTeamResultID.Text = selectedTeamResult.TeamResultID.ToString();
            //Sets the combo boxes to the entries that match the value associated with the 
            //provided Id(primary key). Remember to use selected value, not selected index.
            cboEvent.SelectedValue = selectedTeamResult.EventID;
            cboGame.SelectedValue = selectedTeamResult.GameID;
            cboTeam.SelectedValue = selectedTeamResult.TeamID;
            cboOppositeTeam.SelectedValue = selectedTeamResult.OppositeTeamID;

            cboResult.Text = selectedTeamResult.Result.ToString();
            //Set the save mode to update.
            isNewEntry = false;
        }

        private void btnNewWindow_Click(object sender, RoutedEventArgs e)
        {

            GameResults GameResultsWindow = new GameResults();

            GameResultsWindow.Show();

        }
        private void cboResult_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {
            ComboBox cboResult = (ComboBox)sender;
            if (cboResult.SelectedItem != null)
            {
                ComboBoxItem selectedComboBoxItem = cboResult.SelectedItem as ComboBoxItem;
                string selectedOption = selectedComboBoxItem.Content.ToString();

                switch (selectedOption)
                {
                    case "Win":
                        rbnWinner.IsChecked = true;
                        break;
                    case "Draw":
                        rbnDraw.IsChecked = true;
                        break;
                    case "Opponent Win":
                        rbnOpponentWinner.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTeam.SelectedItem != null && cboOppositeTeam.SelectedItem != null)
            {
                int selectedTeamID = cboTeam.SelectedIndex;
                int selectedOpponentID = cboOppositeTeam.SelectedIndex;

                if (selectedTeamID == selectedOpponentID)
                {
                    MessageBox.Show("Can't selected the same Team");
                    cboTeam.SelectedIndex = -1;
                }
            }
        }


    }
}
