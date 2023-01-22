using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_Tracker.Classes
{
    public class SummaryStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _totDist;
        private string _avgDist;
        private string _totTime;
        private string _avgTime;

        public string totDist
        {
            get { return _totDist; }
            set
            {
                if (this._totDist != value)
                {
                    this._totDist = value;
                    this.NotifyPropertyChanged("totDist");
                }
            }
        }
        public string avgDist
        {
            get { return _avgDist; }
            set
            {
                if (this._avgDist != value)
                {
                    this._avgDist = value;
                    this.NotifyPropertyChanged("avgDist");
                }
            }
        }
        public string totTime
        {
            get { return _totTime; }
            set
            {
                if (this._totTime != value)
                {
                    this._totTime = value;
                    this.NotifyPropertyChanged("totTime");
                }
            }
        }
        public string avgTime
        {
            get { return _avgTime; }
            set
            {
                if (this._avgTime != value)
                {
                    this._avgTime = value;
                    this.NotifyPropertyChanged("avgTime");
                }
            }
        }

        public SummaryStats()
        {

        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }



    }
}
