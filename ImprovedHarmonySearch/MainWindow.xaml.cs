using System;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using System.Linq;
using System.Text;
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
using System.Runtime.InteropServices;




namespace ImprovedHarmonySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        System.IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");
        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

        Random rand = new Random(); // dla pewności rand musi być globalny (jesli się okaze ze nie wystepuje nigdzie ponowna 
        //jego instancja w innych klasach to mozna go przenisc do klasy IHS (Search Harmony) 
        int variablesCount;
        const int n = 5; //maksymalna liczba zmiennych z założeń
        string values; //joined values with "," 
        Function f; //funkcja celu 
        org.mariuszgromada.math.mxparser.Expression ex; //wyrazenie do obliczenia
        List<string> variableNames; //lista nazw zmiennych 

        //parameters of algorithm 
        double hmcr;
        double parMIN;
        double parMAX;
        double bwMIN;
        double bwMAX;
        int NI;
        int HMS;

        double[] xL;
        double[] xU;
        double[][] HM;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchHarmony(object sender, RoutedEventArgs e)
        {
            InitializeParameters();
            HM = InitializeHM();
            ResizeHarmonyMemoryAddFx();
            SortByFx(); 
           
        }

        private void InitializeParameters()
        {
            hmcr = double.Parse(HMCR.Text);
            parMIN = double.Parse(parMin.Text);
            parMAX = double.Parse(parMax.Text);
            bwMIN = double.Parse(bwMin.Text);
            bwMAX = double.Parse(bwMax.Text);
            NI = int.Parse(Ni.Text);
            HMS = int.Parse(hms.Text);

            //inicjalizacja lower and upper bound
            //for (int i = 1; i < variablesCount + 1; i++)
            //{
            //    xL[i] = double.Parse(($"x{i}min.Text");
            //    xU[i] = double.Parse((TextBox)$"x{i}max.Text");
            //}  ----> to jest źle nie działa ale trzeba zautomatyzować proces zczytywania z textboxów 

            xL = new double[]
            {
                double.Parse(x1min.Text),
                double.Parse(x2min.Text),
                // double.Parse(x3min.Text) itp dla n > 2 
            };

            xU = new double[]
            {

                double.Parse(x1max.Text),
                double.Parse(x2max.Text),
                // double.Parse(x3min.Text) itp dla n > 2 
            };
        }

        private void SortByFx()
        {
            HM = HM.OrderBy(x => x[variablesCount]).ToArray();
        }

        private double[][] InitializeHM()
        {
            double[][] HM = new double[HMS][]; //+1 bo jeszcze wartosc funkcji celu  
            

            for (int z = 0; z < HMS; z++)
            {
                HM[z] = new double[variablesCount];
            }

            for (int j = 0; j < variablesCount; j++)
            {
                for (int i = 0; i < HMS; i++) //najpierw inicjalizacja dla całego x1, później dla x2, bo zakresy moga sie roznic 
                {
                    HM[i][j] = RandomNumberInScope(xL[j], xU[j]);
                    
                }
            }
                     

            return HM;
        }

        private void ResizeHarmonyMemoryAddFx()
        {
            double[] fV = InitializeFxValue();
            //powiekszenie tablicy o 1 rozmiar dla wartości  funkcji celu 
            for (int k = 0; k < HMS; k++)
            {
                Array.Resize(ref HM[k], variablesCount + 1); //zwieksz rozmiar 
                HM[k][variablesCount] = fV[k];               //postaw wartosc funkcji celu    
            }
        }

        private double RandomNumberInScope(double lowerBound, double upperBound)
        {
            double random = (rand.NextDouble() * (upperBound - lowerBound)) + lowerBound; //nextdouble to z uniform [0,1]  

            return random;
        }
        
        private double[] InitializeFxValue()
        {
            double[] fValue = new double[HMS];

            for (int i = 0; i < HMS; i++)
            {
                fValue[i] = CalculateObjectiveFunc(HM[i]);
            }

            return fValue;
        }
        // xArr to tablica wartości zmiennych x1,x2 ... 
        private double CalculateObjectiveFunc(double[] xArr)
        {
            double result;

            string spr = string.Join(",", xArr);
            ex = new org.mariuszgromada.math.mxparser.Expression($"f({string.Join(",", xArr)})", f);
            result = ex.calculate();

            return result;
        }

        //step 3 of algorithm 
        private void ImproviseNewHarmony()
        {

        }

        private void lostFocusOnObjFunc(object sender, RoutedEventArgs e)
        {
            var expression = objectiveFunction.Text;
            detectedVar.Text = string.Empty;
            variableNames = new List<string>(); //list of variables name 
          //  XScopeWindow window = new XScopeWindow();

            //wyszukanie liczby zmiennych n <= 5 z załozenia od pani Doktor 
            for (int i = 1; i < n + 1; i++)
            {
                if (expression.Contains($"x{i}"))
                {
                    variablesCount++;
                    variableNames.Add($"x{i}");
                }
            }

            values = string.Join(",", variableNames);
            detectedVar.Text = values;

            f = new Function($"f({values}) = {expression}");
            double[] db = new double[2] { 1, 5 };

            result.Text = $"{CalculateObjectiveFunc(db)}";

            //window.Show();

        }


    }

}

