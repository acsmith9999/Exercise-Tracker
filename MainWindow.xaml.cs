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

        private CollectionViewSource cvs = new CollectionViewSource();


        public MainWindow()
        {
            InitializeComponent();
            SortActivities();

            //SummaryStats();
        }

        #region Loading
        public void SortActivities()
        {
            // sort by date
            List<Activity> sorted = allActivities.OrderByDescending(x => x.ActivityDate).ToList();
            //lvActivities.ItemsSource = sorted;
            cvs.Source = sorted;
            CollectionViewSource.GetDefaultView(sorted).Refresh();
            Binding b = new Binding();
            b.Source = cvs;
            BindingOperations.SetBinding(lvActivities, ListView.ItemsSourceProperty, b);
            //lvActivities.ItemsSource = (System.Collections.IEnumerable)cvs.Source;
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
        #region Filtering
        private void btnSearchDates_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null)
            {
                UserFilter(sender, e);
            }
            else
            {
                MessageBox.Show("Please select a date range!", "Dates?", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void cboActivityType_DropDownClosed(object sender, EventArgs e)
        {
            UserFilter(sender, e);
        }
        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            UserFilter(sender, e);
            
        }
        private void UserFilter(object sender, EventArgs args)
        {
            var b = sender as Control;
            switch (b.Name)
            {
                case "cboFilterType":
                    cvs.Filter += FilterType;
                    break;
                case "btnFilterDates":
                    cvs.Filter += DateRangeFilter;
                    break;
                case "Refresh":
                    cvs.Filter -= FilterType;
                    cvs.Filter -= DateRangeFilter;
                    cboFilterType.SelectedItem = null;
                    break;
            }
        }
        private void FilterType(object sender, FilterEventArgs e)
        {
            Activity activity = e.Item as Activity;
            if (activity.ActivityType.ToString() != cboFilterType.Text.ToString())
            {
                e.Accepted = false;
            }
            else
            {
                //Commented out allows adding multiple filters
                //e.Accepted = true;
            }
        }
        private void DateRangeFilter(object sender, FilterEventArgs e)
        {
            Activity activity = e.Item as Activity;
            if (activity.ActivityDate < startDate || activity.ActivityDate >= endDate)
            {
                e.Accepted = false;
            }
            else
            {
                //e.Accepted = false;
            }
        }

        #endregion

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = (DateTime)dpStartDate.SelectedDate;
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = (DateTime)dpEndDate.SelectedDate;
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

        //TODO - Change combobox to checkcombobox
        private void CheckBoxSelected(object sender, RoutedEventArgs e)
        {
            var checkedTypes = new HashSet<string>();
            foreach (CheckBox checkBox in cboFilterType.Items)
            {
                if (checkBox.IsChecked == true)
                {
                    checkedTypes.Add((string)checkBox.Content);
                }
            }
            cvs.Filter += FilterType;
            //cvs.Filter =
            //job => checkedEmployees.Contains((job as JobModel).EmployeeName);
        }
    }
    
}
