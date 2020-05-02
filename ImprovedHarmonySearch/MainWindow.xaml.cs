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
using System.Diagnostics;

namespace ImprovedHarmonySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string OBJ_FUNC_WRONG = "Wrong format of objective function!";
        const string OBJ_FUNC_VARIABLE_NOT_FOUND = "Parser does not find any variables.";

        Random rand = new Random(); 
        int decisionVariableQty; //in algorithm just N 
        const int n = 5; //max of decision variables 
        string values; //joined values with "," 
        private static Function function; //objective function
        string expression;
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

        double[] newImprovisedHarmony;
        int gn = 0;
        double PARgn;
        double bwgn;
        double c;
        int D1;
        double D2;
        double D3;
        double fx;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoSearchHarmonyOnClick(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AmelioratedHarmonySearch ahs = new AmelioratedHarmonySearch(values, expression, decisionVariableQty,
                                                                        xL, xU, hmcr, parMIN, parMAX, bwMIN,
                                                                        bwMAX, NI, HMS);
            ahs.ImprovedHarmonySearch();
            string results = ahs.GetResults();

            sw.Stop();

            result.Text = results;
            //  result.Text = $"time: {sw.ElapsedMilliseconds}";
            CountBtn.IsEnabled = false;

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


            xL = new double[]
            {
                double.Parse(x1min.Text),
                double.Parse(x2min.Text),
              //  double.Parse(x3min.Text),// itp dla n > 2 
               // double.Parse(x4min.Text)
            };

            xU = new double[]
            {
                double.Parse(x1max.Text),
                double.Parse(x2max.Text),
             //   double.Parse(x3max.Text), //itp dla n > 2 
             //   double.Parse(x4max.Text)
            };

        }

        private void lostFocusOnObjFunc(object sender, RoutedEventArgs e)
        {
            expression = objectiveFunction.Text;

            detectedVariables.Text = string.Empty;
            decisionVariableQty = 0;

            List<string> variables = DetectVariables(expression);

            decisionVariableQty = variables.Count;

            if (decisionVariableQty != 0)
            {
                values = string.Join(",", variables);
                detectedVariables.Text = values;
                function = new Function($"f({values}) = {expression}");

                //check syntax of obj function 
                if (function.checkSyntax() == false)
                {
                    MessageBox.Show(OBJ_FUNC_WRONG);
                }
            }
            else
            {
                MessageBox.Show(OBJ_FUNC_VARIABLE_NOT_FOUND);
            }

        }

        private List<string> DetectVariables(string expression)
        {
            List<string> variables = new List<string>();

            for (int i = 1; i < n + 1; i++)
            {
                if (expression.Contains($"x{i}"))
                {
                    variables.Add($"x{i}");
                }
            }

            return variables;
        }

        private void SaveParametersOnclick(object sender, RoutedEventArgs e)
        {
            if (CheckParameters() == true)
            {
                CountBtn.IsEnabled = true;
                InitializeParameters();
            }
        }

        private bool CheckParameters()
        {
            if (double.Parse(parMax.Text) >= 0 && double.Parse(parMax.Text) <= 1
             && double.Parse(parMin.Text) >= 0 && double.Parse(parMin.Text) <= 1
             && double.Parse(parMin.Text) < double.Parse(parMax.Text)
             && double.Parse(HMCR.Text) >= 0 && double.Parse(HMCR.Text) <= 1
             && double.Parse(bwMin.Text) >= 0 && double.Parse(bwMax.Text) >= 0
             && double.Parse(bwMin.Text) < double.Parse(bwMax.Text)
             && int.Parse(Ni.Text) > 0 && int.Parse(hms.Text) > 0)
                return true;

            return false;
        }

    }

}

