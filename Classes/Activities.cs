using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace Exercise_Tracker.Classes
{
    public sealed class Activities: ObservableCollection<Activity>
    {
        #region Constructors
        public Activities()
        {
            GetAllActivities();
        }
        public Activities(DateTime startDate, DateTime endDate)
        {
            GetAllActivities(startDate, endDate);
        }
        public Activities(string type)
        {
            GetAllActivities(type);
        }
        #endregion

        #region Public Methods
        public void Refresh()
        {
            this.Clear();
            GetAllActivities();
        }
        #endregion

        #region Private Methods
        private void GetAllActivities()
        {
            SqlDAL myDAL = new SqlDAL();
            DataTable activitiesTable = myDAL.ExecuteStoredProc("usp_GetAllActivities");

            foreach(DataRow activityRow in activitiesTable.Rows)
            {
                Activity activity = new Activity(activityRow);
                Add(activity);
            }
        }
        private void GetAllActivities(DateTime startDate, DateTime endDate)
        {
            SqlDAL myDAL = new SqlDAL();
            SqlParameter[] parameters =
            {
                new SqlParameter("@startDate", startDate),
                new SqlParameter("@endDate", endDate)
            };

            DataTable activitiesTable = myDAL.ExecuteStoredProc("usp_GetActivitiesInDateRange", parameters);

            foreach (DataRow activityRow in activitiesTable.Rows)
            {
                Activity activity = new Activity(activityRow);
                Add(activity);
            }
        }
        private void GetAllActivities(string type)
        {
            SqlDAL myDAL = new SqlDAL();
            SqlParameter[] parameters =
            {
                new SqlParameter("@activityType", type)
            };

            DataTable activitiesTable = myDAL.ExecuteStoredProc("usp_GetActivitiesByType", parameters);

            foreach (DataRow activityRow in activitiesTable.Rows)
            {
                Activity activity = new Activity(activityRow);
                Add(activity);
            }
        }
        #endregion
    }
}
