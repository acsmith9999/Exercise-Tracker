using Exercise_Tracker.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Activities allActivities = new Activities();
        public Activity selectedActivity;
        private DateTime startDate;
        private DateTime endDate;

        public MainWindow()
        {
            InitializeComponent();
            LoadActivities();
        }

        #region Loading
        public void LoadActivities()
        {
            allActivities.Clear();
            allActivities = new Activities();
            // sort by date
            Comparison<Activity> compareDate = new Comparison<Activity>(Activity.CompareDate);
            allActivities.Sort(compareDate);
            
            lvActivities.ItemsSource = allActivities.Reverse<Activity>();

            SummaryStats();
        }
        #endregion
        #region ButtonClicks
        private void lvActivities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnUpdateActivity_Click(object sender, RoutedEventArgs e)
        {
            
            if (lvActivities.SelectedItem != null)
            {
                selectedActivity = (Activity)lvActivities.SelectedItem;

                UpdateActivity createWindow = new UpdateActivity();
                Application.Current.MainWindow.IsEnabled = false;
                createWindow.Show();

            }
            else { MessageBox.Show("Please select an activity to edit", "Select Activity!", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
        }

        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            AddActivity createWindow = new AddActivity();
            Application.Current.MainWindow.IsEnabled = false;
            createWindow.Show();
        }

        private void btnDeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        #region sorting
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender,
                                                RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvActivities.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion

        private void btnSearchDates_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null)
            {
                allActivities = new Activities(startDate, endDate);
                // sort by date
                Comparison<Activity> compareDate = new Comparison<Activity>(Activity.CompareDate);
                allActivities.Sort(compareDate);

                lvActivities.ItemsSource = allActivities.Reverse<Activity>();
                SummaryStats();
            }
            else
            {
                MessageBox.Show("Please select a date range!", "Dates?", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = (DateTime)dpStartDate.SelectedDate;
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = (DateTime)dpEndDate.SelectedDate;
        }

        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadActivities();
        }

        private void cboActivityType_DropDownClosed(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvActivities.ItemsSource);
            view.Filter = UserFilter;

            CollectionViewSource.GetDefaultView(lvActivities.ItemsSource).Refresh();

        }
        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(cboActivityType.Text))
                return true;
            else
                return (item as Activity).ActivityType.IndexOf(cboActivityType.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        private void SummaryStats()
        {
            //TODO - Make INotifyPropertyChanged 
            decimal totDist = 0;
            TimeSpan totTime = new TimeSpan();

            for (var i= 0; i< lvActivities.Items.Count; i++)
            {
                Activity temp = lvActivities.Items[i] as Activity;
                totDist += temp.Distance;
                totTime+=TimeSpan.Parse(temp.Duration);
            }
            decimal avgDist = totDist / lvActivities.Items.Count;
            TimeSpan avgTime = new TimeSpan(totTime.Ticks/ lvActivities.Items.Count);
            txtTotalDist.Text = totDist.ToString("F");
            txtAvgDist.Text = avgDist.ToString("F");
            txtTotalTime.Text = totTime.ToString(@"hh\:mm\:ss");
            txtAvgTime.Text = avgTime.ToString(@"hh\:mm\:ss");
        }
    }
}
