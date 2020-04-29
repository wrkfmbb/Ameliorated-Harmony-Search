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

namespace ImprovedHarmonySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random(); // dla pewności rand musi być globalny (jesli się okaze ze nie wystepuje nigdzie ponowna 
        //jego instancja w innych klasach to mozna go przenisc do klasy IHS (Search Harmony) 
        int variablesCount = new int(); 
        const int n = 5; //maksymalna liczba zmiennych z założeń
        string values; //joined values with "," 
        Function f; //funkcja celu 
        org.mariuszgromada.math.mxparser.Expression ex; //wyrazenie do obliczenia
        List<string> variableNames; //lista nazw zmiennych 


        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchHarmony(object sender, RoutedEventArgs e)
        {
            // var value = objectiveFunction.Text;
            // result.Text = value;
            //double[] rndW = new double[6];
            //double[] rndG = new double[6]; 
            //for (int i = 0; i < 6; i ++)
            //{
            //                 rndG[i] = rand.NextDouble()*(2-(-2)) - 2; //nextdouble to z uniform 
            //}

        }

        private void lostFocusOnObjFunc(object sender, RoutedEventArgs e)
        {
            var expression = objectiveFunction.Text;
            detectedVar.Text = string.Empty;
            variableNames = new List<string>(); //list of variables name 

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
            double[] db = new double[2] { 1,5};

            result.Text = $"{CalculateObjectiveFunc(db)}";
        }


        // xArr to tablica wartości zmiennych x1,x2 ... 
        private double CalculateObjectiveFunc(double[] xArr)
        {
            double result;

            ex = new org.mariuszgromada.math.mxparser.Expression($"f({string.Join(",", xArr)})", f);
            result = ex.calculate();

            return result;
        }

    }
}
