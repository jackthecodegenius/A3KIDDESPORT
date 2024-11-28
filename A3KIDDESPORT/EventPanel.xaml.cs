using DataManagement.Models;
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
using System.Text.RegularExpressions;


namespace A3KIDDESPORT
{
    /// <summary>
    /// Interaction logic for EventPanel.xaml
    /// </summary>
    public partial class EventPanel : UserControl
    {
        

        // Create our class object for communicating with the database. 
        DataAdapter data = new DataAdapter();
        // A list of User objects.
        List<Event> eventList = new List<Event>();
        //Acts as a flag to indicate which way to save our data, as a new entry or an edit.
        bool isNewEntry = true;

        public EventPanel()
        {
            InitializeComponent();
            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            eventList = data.GetAllEvents();
            dgvEvent.ItemsSource = eventList;
            dgvEvent.Items.Refresh();
        }

        private void ClearDataEntryFields()
        {
            txtEventID.Text = string.Empty;
            txtEventName.Text = string.Empty;
            txtEventLocation.Text = string.Empty;
            pkrDate.SelectedDate = DateTime.Today;

            //Sets the save flag to new entry mode.
            isNewEntry = true;
        }

        private bool IsFormFilledCorrectly()
        {
            if (String.IsNullOrEmpty(txtEventName.Text))
            {
                return false;
            }
            if (String.IsNullOrEmpty(txtEventLocation.Text))
            {
                return false;
            }
            if (pkrDate.SelectedDate == null || pkrDate.SelectedDate > DateTime.Now)
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
                MessageBox.Show("Please ensure form is filled correctly before saving!\n" +
                                "-Ensure all sections are filled/selected\n" +
                                "-Ensure date selected is not a future date");
                return;
            }

            // Get the user details from the entry form
            Event EventEntry = new Event();

            EventEntry.EventName = txtEventName.Text;
            EventEntry.EventLocation = txtEventLocation.Text;
            EventEntry.EventDate = pkrDate.SelectedDate.Value;



            //Chooses the desired save mode based upon the state of the isNewEntry flag.
            if (isNewEntry)
            {
                //Pass the user details to the database to be added.
                data.AddNewEvent(EventEntry);
            }
            else
            {
                // Get the user Id from the entry form.
                EventEntry.EventID = int.Parse(txtEventID.Text);
                // Pass the user details to the database to be updated.
                data.UpdateEvent(EventEntry);
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


            if (dgvEvent.SelectedIndex < 0)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Delete {eventList[dgvEvent.SelectedIndex].EventName}?",
                                                        "Delete Confirmation",
                                                         MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                int id = eventList[dgvEvent.SelectedIndex].EventID;
                data.DeleteEvent(id);
                MessageBox.Show("Record Deleted.");
                UpdateDataGrid();
                ClearDataEntryFields();
            }
           
        }

        private void dgvEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Checks that a vlid row is selected, otherwise it returns out of the method.
            if (dgvEvent.SelectedIndex < 0)
            {
                return;
            }
            //Get the Id of the selected entry from the list.
            int EventID = eventList[dgvEvent.SelectedIndex].EventID;
            //Gets the user from the database that matches the current Id value. 
            Event EventEntry = data.GetEventByEventID(EventID);

            if (EventEntry == null)
            {
                MessageBox.Show("Something went wrong! \nPlease Try Again");
                UpdateDataGrid();
                return;
            }
            //Copy the user details into the form.
            txtEventID.Text = EventEntry.EventID.ToString();
            txtEventName.Text = EventEntry.EventName;
            txtEventLocation.Text = EventEntry.EventLocation;
            pkrDate.SelectedDate = EventEntry.EventDate;

            //Sets the save flag to edit mode.
            isNewEntry = false;
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
