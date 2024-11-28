using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Window
    {
        //Databse connection object to manage all data interactions.
        DataAdapter data = new DataAdapter();
        //List of options for reports combo box selction.
        List<string> reportOptions = new List<string> { "TeamDetails By Competition Points", "TeamResults By Events", "TeamResults By Team" };

        //Lists to manage the filtering functionality of the Team reports. The full list holds all the entire records when initially retrieved from database.
        //The filters list is the state of the collection after applying the current filters.
        //The displaylist is the list used by the Data Grid to display whichever of the 2 lists is currently being used. This list holds no data of its own and is just
        //used keep a reference of the currently desired list when it is assigned.
        List<TeamDetail> teamList;
        List<TeamDetail> filteredteamList = new List<TeamDetail>();
        List<TeamDetail> customerteamList;

        //Lists to manage the filtering functionality of the Team Results reports. The full list holds all the entire records when initially retrieved from database.
        //The filters list is the state of the collection after applying the current filters.
        //The displaylist is the list used by the Data Grid to display whichever of the 2 lists is currently being used. This list holds no data of its own and is just
        //used keep a reference of the currently desired list when it is assigned.
        List<ResultView> resultViewList;
        List<ResultView> filteredresultViewList = new List<ResultView>();
        List<ResultView> displayresultViewList;


        public ReportPage()
        {
            InitializeComponent();

            //Assigns the list of options to the combo box.
            cboType.ItemsSource = reportOptions;

        }

        /// <summary>
        /// Manages the functionality triggered when the option selected in the combo box is changed. 
        /// </summary>
      
        private void cboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If the combo box is on the empty selection, do nothing.
            //Otherwise, retrieve and filter the required data according to the desired selection. 
            if (cboType.SelectedIndex < 0)
            {
                return;
            }
            else if (cboType.SelectedIndex == 0)
            {
                //Retrieves the data from the database. 
                teamList = data.GetAllTeamDetails();
                //Using Linq, order the retrieved records by customer Last Name.
                teamList = teamList.OrderBy(s => s.CompetitionPoints).ToList();
                DisplayActiveTeamDetailList(teamList);

            }
            else if (cboType.SelectedIndex == 1)
            {
                //Retrieves the data from the database. 
                resultViewList = data.GetAllTeamResults();
                //Using Linq, order the retrieved records by Product Category.
                resultViewList = resultViewList.OrderBy(c => c.EventHeld).ToList();
                DisplayActiveTeamResultList(resultViewList);
            }
            else
            {
                //Retrieves the data from the database. 
                resultViewList = data.GetAllTeamResults();
                //Using Linq, order the retrieved records in descending order by product Price. In this instance the price needs to be
                //changed from a string to integer after removing the dollar sign from the price.
                resultViewList = resultViewList.OrderBy(c => c.Team).ToList();
                DisplayActiveTeamResultList(resultViewList);
            }

            //Clear the text field.
            txtSearch.Text = "";

        }

        /// <summary>
        /// Display the product list on screen according to whichever version of the product list is desired(full or filtered) 
        /// </summary>
        /// <param name="activeList"> The list to be shown in the Data Grid</param>
        private void DisplayActiveTeamResultList(List<ResultView> activeList)
        {
            displayresultViewList = activeList;
            dgvReport.ItemsSource = displayresultViewList;
            dgvReport.Items.Refresh();
        }

        /// <summary>
        /// Event triggered when the Export button is pressed.
        /// </summary>
        /// <param name="sender">The object triggering the event</param>
        /// <param name="e">Any paramaters passed when the event is triggered by its component</param>
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //If no eport type is selected, do nothing and return.
            if (cboType.SelectedIndex < 0)
            {
                return;
            }

            //Create save dialog object. This will eventually open the Windows file chooser to allow file name and location selection.
            SaveFileDialog saveDialog = new SaveFileDialog();
            //Sets the filters for the allowed file types of the dialog.
            //These are passed as a string in the following format: {File Description}|{extension type} with each fileter separated by a new pipe(|) character.
            saveDialog.Filter = "Simple Text File(.txt)|*.txt|" +
                                "Comma Separated Values (.csv)|*.csv";
            //Sets the initial file directory when the dialog first opens. This version retrieves the filepath for the desktop directory.
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //If the dialog returns a true message(OK/SAVE button pressed) proceed with the saving procedure.
            //The Show dialog process wihtin the if statement, opens the file chooser.
            if (saveDialog.ShowDialog() == true)
            {
                //Save the data in comma separated format based uponn whether the selecter is on the customer(0 index) or product ( 1+ indexes).
                if (cboType.SelectedIndex == 0)
                {
                    //Create stream writer to manage writing to file.
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        //Iterate through each item in the displayed customer list and write them to file.
                        foreach (var item in customerteamList)
                        {
                            writer.WriteLine($"{item.TeamID},{item.TeamName},{item.PrimaryContact},{item.ContactPhone},{item.ContactEmail},{item.CompetitionPoints}");
                        }
                    }
                }
                else
                {
                    //Create stream writer to manage writing to file.
                    using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                    {
                        //Iterate through each item in the displayed customer list and write them to file.
                        foreach (var item in displayresultViewList)
                        {
                            writer.WriteLine($"{item.ResultViewID},{item.EventHeld},{item.GamesPlayed},{item.Team}," +
                                              $"{item.OpposingTeam},{item.Result}");
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Event which triggers when the text in the search text field changes.
        /// </summary>

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //If no report type is selected, do nothing.
            if (cboType.SelectedIndex < 0)
            {
                return;
            }

            //If the customer option is selected in the combo box (Index 0), filter team list.
            if (cboType.SelectedIndex == 0)
            {
                //If no text is in the search field, display the full list.
                if (String.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    DisplayActiveTeamDetailList(teamList);
                    return;
                }

                //Filter list using Linq to copy all the team names contain contents of the search field.
                filteredteamList = teamList.Where(f => f.TeamName.ToUpper().Contains(txtSearch.Text.ToUpper())
                                                         || f.TeamName.ToUpper().Contains(txtSearch.Text.ToUpper())).ToList();

                DisplayActiveTeamDetailList(filteredteamList);
            }

            //If one of the product options is selected in the combo box (Indexes 1+), filter the team result list.
            if (cboType.SelectedIndex > 0)
            {
                //If no text is in the search field, display the full list.
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    DisplayActiveTeamResultList(resultViewList);
                    return;
                }
                //Filter list using Linq to copy all the products whos  names or description field contains contents of the search field.
                filteredresultViewList = resultViewList.Where(p => p.Team.ToUpper().Contains(txtSearch.Text.ToUpper()) ||
                                                              p.Team.ToUpper().Contains(txtSearch.Text.ToUpper())).ToList();


                DisplayActiveTeamResultList(filteredresultViewList);
            }


        }

        /// <summary>
        /// Display the team list on screen according to whichever version of the team list is desired(full or filtered) 
        /// </summary>
        /// <param name="activeList"> The list to be shown in the Data Grid</param>
        private void DisplayActiveTeamDetailList(List<TeamDetail> activeList)
        {
            teamList = activeList;
            dgvReport.ItemsSource = teamList;
            dgvReport.Items.Refresh();
        }

        /// <summary>
        /// Event when close button is pressed to close the window.
        /// </summary>

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
