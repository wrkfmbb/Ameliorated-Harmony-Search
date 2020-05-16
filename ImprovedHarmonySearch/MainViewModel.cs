using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImprovedHarmonySearch
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.MyModel = new PlotModel();
        }

        private PlotModel _myModel;
        public PlotModel MyModel
        {
            get { return _myModel; }
            set
            {
                if (value != _myModel)
                {
                    _myModel = value;
                    OnPropertyChanged();
                }
            }
        }

    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String propName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
