using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;

namespace Exercise_Tracker.Classes
{
    public sealed class Activity :INotifyPropertyChanged
    {
        private int _activityId;
        private string _activityType;
        private DateTime _activityDate;
        private string _duration;
        private decimal _distance;
        #region Public Properties
        public int ActivityId
        {
            get { return _activityId; } 
            set
            {
                if (this._activityId != value)
                {
                    this._activityId = value;
                    this.NotifyPropertyChanged("Id");
                }
            }
        }
        public string ActivityType
        {
            get { return this._activityType; }
            set
            {
                if (this._activityType != value)
                {
                    this._activityType = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }
        public DateTime ActivityDate
        {
            get { return this._activityDate; }
            set
            {
                if (this._activityDate != value)
                {
                    this._activityDate = value;
                    this.NotifyPropertyChanged("Date");
                }
            }
        }
        public String Duration
        {
            get { return this._duration; }
            set
            {
                if (this._duration != value)
                {
                    this._duration = value;
                    this.NotifyPropertyChanged("Duration");
                }
            }
        }
        public decimal Distance
        {
            get { return this._distance; }
            set
            {
                if (this._distance != value)
                {
                    this._distance = value;
                    this.NotifyPropertyChanged("Distance");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        public Activity()
        {

        }

        public Activity(int activityId, string activityType, DateTime activityDate, string duration, decimal distance)
        {
            this.ActivityId = activityId;
            this.ActivityType = activityType;
            this.ActivityDate = activityDate;
            this.Duration = duration;
            this.Distance = distance;
        }

        public Activity(DataRow activityRow)
        {
            ActivityId = (int)activityRow["Id"];
            ActivityType = activityRow["Type"].ToString();
            ActivityDate = (DateTime)activityRow["Date"];
            Duration = activityRow["Duration"].ToString();
            Distance = (decimal)activityRow["Distance"];
        }

        #endregion


        #region Public Data Methods
        public int AddNewActivity()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Type",ActivityType),
                    new SqlParameter("@Date",ActivityDate),
                    new SqlParameter("@Duration",Duration),
                    new SqlParameter("@Distance",Distance)
                };

                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_AddNewActivity", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The activity could not be added!\n", ex);
            }
        }

        public int RemoveActivity()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@ActivityId", ActivityId)
                };

                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_DeleteActivity", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The activity could not be deleted!\n", ex);
            }
        }

        public int UpdateActivity(Activity selectedActivity)
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Type",selectedActivity.ActivityType),
                    new SqlParameter("@Date",selectedActivity.ActivityDate),
                    new SqlParameter("@Duration",selectedActivity.Duration),
                    new SqlParameter("@Distance",selectedActivity.Distance),
                    new SqlParameter("@Id",selectedActivity.ActivityId)
                };
                //convert date value for DOB
                //parameters[1].SqlDbType = SqlDbType.DateTime;
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_UpdateActivity", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The activity could not be updated! ", ex);
            }
        }
        #endregion

        #region Public Methods
        public static int CompareDate(Activity a1, Activity a2)
        {
            return a1.ActivityDate.CompareTo(a2.ActivityDate);
        }
        public void NotifyPropertyChanged(string propName)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        #endregion

    }
}
