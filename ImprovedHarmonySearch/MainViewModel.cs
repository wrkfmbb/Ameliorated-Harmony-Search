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

            double x0 = -6;
            double x1 = 6;
            double y0 = -3;
            double y1 = 3;

            //generate values
            Func<double, double, double> peaks = (x, y) => x+y-x-y;//3 * (1 - x) * (1 - x) * Math.Exp(-(x * x) - (y + 1) * (y + 1)) - 10 * (x / 5 - x * x * x - y * y * y * y * y) * Math.Exp(-x * x - y * y) - 1.0 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            var xx = ArrayBuilder.CreateVector(x0, x1, 100);
            var yy = ArrayBuilder.CreateVector(y0, y1, 100);
            var peaksData = ArrayBuilder.Evaluate(peaks, xx, yy);

            var cs = new ContourSeries
            {
                Color = OxyColors.Black,
                LabelBackground = OxyColors.Transparent,
                ColumnCoordinates = yy,
                RowCoordinates = xx,
                Data = peaksData
            };
            this.MyModel.Series.Add(cs);

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
