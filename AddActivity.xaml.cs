using Exercise_Tracker.Classes;
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

namespace Exercise_Tracker
{
    /// <summary>
    /// Interaction logic for AddActivity.xaml
    /// </summary>
    public partial class AddActivity : Window
    {
        MainWindow feed = (MainWindow)Application.Current.MainWindow;
        public AddActivity()
        {
            InitializeComponent();
        }

        private void cboActivityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            //do the save
            if (ValidateInputControls())
            {
                Activity newActivity = new Activity();

                AssignPropertiesToActivity(newActivity);

                if (newActivity.AddNewActivity() == 1)
                {
                    MessageBox.Show("New activity added");
                    Close();
                    feed.IsEnabled = true;
                    feed.SortActivities();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            string message = "All changes will be lost! \n Do you wish to continue?";
            string caption = "Cancel add new?";

            if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Close();
                feed.IsEnabled = true;
                feed.SortActivities();
            }
        }
        private bool ValidateInputControls()
        {
            if (cboActivityType.SelectedItem == null || cboActivityType.Text == null)
            {
                MessageBox.Show("Please enter a type!", "Type?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (dpActivityDate.SelectedDate == null)
            {
                MessageBox.Show("Please enter a date!", "Date?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (tpActivityTime.Value == null || tpActivityTime.Text == null)
            {
                MessageBox.Show("Please enter a time!", "Time?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (Convert.ToInt32(txtMinutes.Text) > 59)
            {
                MessageBox.Show("Invalid minutes!", "Minutes?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (Convert.ToInt32(txtSeconds.Text) > 59)
            {
                MessageBox.Show("Invalid seconds!", "Seconds?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void AssignPropertiesToActivity(Activity newActivity)
        {
            DateTime date = Convert.ToDateTime(dpActivityDate.SelectedDate);
            
            DateTime time = (DateTime)tpActivityTime.Value;
            TimeSpan addTime = new TimeSpan(time.Hour, time.Minute, time.Second);
            newActivity.ActivityDate = date + addTime;

            newActivity.ActivityType = cboActivityType.Text;

            //add duration
            TimeSpan ts = new TimeSpan((int)txtHours.Value, (int)txtMinutes.Value, (int)txtSeconds.Value);
            newActivity.Duration = ts.ToString();

            //add dist
            if(!string.IsNullOrEmpty(txtDistance.Text))
            {
                newActivity.Distance = Convert.ToDecimal(txtDistance.Text);
            }

        }

        #region input validation
        //Permits only decimal input in duration and distance fields
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d+");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }
        private static bool IsValid(string str)
        {
            Decimal i;
            return Decimal.TryParse(str, out i) && i >= 0 && i <= 99999 && Decimal.Round(i, 2) == i;
        }
        private void TimeTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"/^(0?[1-9]|1[0-2]):[0-5][0-9]$/");
            e.Handled = !regex.IsMatch(e.Text);
        }

        #endregion
    }
}
