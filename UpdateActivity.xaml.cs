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
using System.Windows.Shapes;

namespace Exercise_Tracker
{

    /// <summary>
    /// Interaction logic for UpdateActivity.xaml
    /// </summary>
    public partial class UpdateActivity : Window
    {
        MainWindow feed = (MainWindow)Application.Current.MainWindow;
        Activity selectedActivity;
        public UpdateActivity()
        {
            InitializeComponent();
            selectedActivity = feed.selectedActivity;
            cboActivityType.Text = selectedActivity.ActivityType.ToString();
            dpActivityDate.SelectedDate = selectedActivity.ActivityDate.Date;
            tpActivityTime.Value = Convert.ToDateTime(selectedActivity.ActivityDate.TimeOfDay.ToString());
            txtHours.Text = TimeSpan.Parse(selectedActivity.Duration).Hours.ToString();
            txtMinutes.Text = TimeSpan.Parse(selectedActivity.Duration).Minutes.ToString();
            txtSeconds.Text = TimeSpan.Parse(selectedActivity.Duration).Seconds.ToString();
            txtDistance.Text = selectedActivity.Distance.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            feed.IsEnabled = true;
            Close();
        }

        private void cboActivityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void btnUpdateActivity_Click(object sender, RoutedEventArgs e)
        {
            string message;

            try
            {
                //do the save
                if (ValidateInputControls())
                {
                    AssignPropertiesToActivity(selectedActivity);

                    if (selectedActivity.UpdateActivity(selectedActivity) == 1)
                    {
                        MessageBox.Show("Update success!");
                        Close();
                        feed.IsEnabled = true;
                        feed.LoadActivities();
                    }
                }
            }
            catch (Exception ex)
            {
                message = $"Something went wrong! \n The activities's details could not be updated. \n{ex.Message}";
                MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (!string.IsNullOrEmpty(txtDistance.Text))
            {
                newActivity.Distance = Convert.ToDecimal(txtDistance.Text);
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            string message = "Are you sure you want to delete? \n The activities's details will be permanently deleted!";
            string caption = "Delete activity?";

            if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    if (selectedActivity.RemoveActivity() == 1)
                    {
                        MessageBox.Show("Activities's details successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                        feed.IsEnabled = true;
                        feed.LoadActivities();
                    }
                }
                catch (Exception ex)
                {
                    message = $"Something went wrong! \n The activities's details could not be deleted. \n{ex.Message}";
                    MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
       
    }
}
