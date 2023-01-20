using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace Exercise_Tracker.Classes
{
    public sealed class Activity 
    {
        #region Public Properties
        public int ActivityId { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
        public String Duration { get; set; }
        public decimal Distance { get; set; }
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
        #endregion

    }
}
