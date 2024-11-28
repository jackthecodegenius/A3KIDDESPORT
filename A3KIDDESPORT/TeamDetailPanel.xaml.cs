using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for TeamDetailPanel.xaml
    /// </summary>
    public partial class TeamDetailPanel : UserControl
    {
      

        // Create our class object for communicating with the database. 
        DataAdapter data = new DataAdapter();
        // A list of User objects.
        List<TeamDetail> teamList = new List<TeamDetail>();
        //Acts as a flag to indicate which way to save our data, as a new entry or an edit.
        bool isNewEntry = true;

        public TeamDetailPanel()
        {
            InitializeComponent();
            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            teamList = data.GetAllTeamDetails();
            dgvTeamDetail.ItemsSource = teamList;
            dgvTeamDetail.Items.Refresh();
        }

        private void ClearDataEntryFields()
        {
            txtTeamID.Text = string.Empty;
            txtTeamName.Text = string.Empty;
            txtPrimaryContact.Text = string.Empty;
            txtContactPhone.Text = string.Empty;
            txtContactEmail.Text = string.Empty;
            txtCompetitionPoints.Text = string.Empty;
            //Sets the save flag to new entry mode.
            isNewEntry = true;
        }

        private bool IsFormFilledCorrectly()
        {
            if (String.IsNullOrEmpty(txtTeamName.Text))
            {
                return false;
            }
            if (String.IsNullOrEmpty(txtPrimaryContact.Text))
            {
                return false;
            }
            if (String.IsNullOrEmpty(txtContactPhone.Text))
            {
                return false;
            }
            if (String.IsNullOrEmpty(txtContactEmail.Text))
            {
                return false;
            }
            if (String.IsNullOrEmpty(txtCompetitionPoints.Text))
            {
               
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("Make sure form fields are filled correctly before trying to save!");
                return;
            }

            // Get the user details from the entry form
            TeamDetail teamDetailEntry = new TeamDetail(); 
            
            teamDetailEntry.TeamName = txtTeamName.Text;
            teamDetailEntry.PrimaryContact = txtPrimaryContact.Text;
            teamDetailEntry.ContactPhone = txtContactPhone.Text;
            teamDetailEntry.ContactEmail = txtContactEmail.Text;
            teamDetailEntry.CompetitionPoints = int.Parse(txtCompetitionPoints.Text);

            if (isNewEntry)
            {
                //Pass the user details to the database to be added.
                data.AddNewTeamDetail(teamDetailEntry);
            }
            else
            {
                // Get the user Id from the entry form.
                teamDetailEntry.TeamID = int.Parse(txtTeamID.Text);
                // Pass the user details to the database to be updated.
                data.UpdateTeamDetail(teamDetailEntry);
            }

        



            //Update the on-screen display.
            ClearDataEntryFields();
            UpdateDataGrid();
        }



        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearDataEntryFields();
        }
        
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgvTeamDetail.SelectedIndex < 0)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Delete {teamList[dgvTeamDetail.SelectedIndex].TeamName}?",
                                                        "Delete Confirmation",
                                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                int id = teamList[dgvTeamDetail.SelectedIndex].TeamID;
                data.DeleteTeamDetail(id);
                MessageBox.Show("Record Deleted.");
                UpdateDataGrid();
                ClearDataEntryFields();
            }

        }


        private void dgvTeamDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Checks that a vlid row is selected, otherwise it returns out of the method.
            if (dgvTeamDetail.SelectedIndex < 0)
            {
                return;
            }
            //Get the Id of the selected entry from the list.
            int TeamID = teamList[dgvTeamDetail.SelectedIndex].TeamID;
            //Gets the user from the database that matches the current Id value. 
            TeamDetail teamDetailEntry = data.GetTeamDetailByTeamID(TeamID);

            if (teamDetailEntry == null)
            {
                MessageBox.Show("Something went wrong! \nPlease Try Again");
                UpdateDataGrid();
                return;
            }
            //Copy the user details into the form.
            txtTeamID.Text = teamDetailEntry.TeamID.ToString();
            txtTeamName.Text = teamDetailEntry.TeamName;
            txtPrimaryContact.Text = teamDetailEntry.PrimaryContact;
            txtContactPhone.Text = teamDetailEntry.ContactPhone;
            txtContactEmail.Text = teamDetailEntry.ContactEmail;
            txtCompetitionPoints.Text = teamDetailEntry.CompetitionPoints.ToString();
            //Sets the save flag to edit mode.
            isNewEntry = false;
        }

        private void dgvTeamDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        // Makes it so you only put numbers into textbox
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void LetterValidation(object sender, TextCompositionEventArgs e)
        {

            Regex regex2 = new Regex(@"^[A-Za-z]*$");
            e.Handled = !regex2.IsMatch(e.Text);

        }
    }
}
