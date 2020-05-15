﻿using System;
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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace ImprovedHarmonySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string OBJ_FUNC_WRONG = "Wrong format of objective function!";
        const string OBJ_FUNC_VARIABLE_NOT_FOUND = "Parser does not find any variables.";
        const string WRONG_PARAMETERS = "Wrong parameters.";

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

        TextBox[] minTextbox;
        TextBox[] maxTextbox;
        TextBlock[] descriptionTextblock;



        public MainWindow()
        {
            InitializeComponent();
            GetTextboxesX();
            HideDesc(); 
            HideAllTextboxes(); 

        }

        void GetTextboxesX()
        {
            minTextbox = new TextBox[] { x1min, x2min, x3min, x4min, x5min };
            maxTextbox = new TextBox[] { x1max, x2max, x3max, x4max, x5max };
            descriptionTextblock = new TextBlock[] { x1, x2, x3, x4, x5 };
        }

        void HideDesc()
        {
            min.Visibility = Visibility.Hidden;
            max.Visibility = Visibility.Hidden;
            Constraints.Visibility = Visibility.Hidden;
        }
        void ShowDesc()
        {
            min.Visibility = Visibility.Visible;
            max.Visibility = Visibility.Visible;
            Constraints.Visibility = Visibility.Visible;
        }

        void HideAllTextboxes()
        {
            for (int i = 0; i < n; i++)
            {
                minTextbox[i].Visibility = Visibility.Hidden;
                maxTextbox[i].Visibility = Visibility.Hidden;
                descriptionTextblock[i].Visibility = Visibility.Hidden;
            }
         
        }

        void SetVisibleTextbox()
        {
            HideAllTextboxes();
            ShowDesc(); 
            for (int i = 0; i < decisionVariableQty; i++)
            {
                minTextbox[i].Visibility = Visibility.Visible;
                maxTextbox[i].Visibility = Visibility.Visible;
                descriptionTextblock[i].Visibility = Visibility.Visible;
            }
        }

        private void DoSearchHarmonyOnClick(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AmelioratedHarmonySearch ahs = new AmelioratedHarmonySearch(values, expression, decisionVariableQty,
                                                                        xL, xU, hmcr, parMIN, parMAX, bwMIN,
                                                                        bwMAX, NI, HMS);
            ahs.ImprovedHarmonySearch();

            string[] results = ahs.GetResults();

            sw.Stop();

            string[] outcome = new string[results.Length];

            for (int i = 0; i < decisionVariableQty; i++)
            {
                outcome[i] = $"x{i + 1} = {results[i]}";
            }
            outcome[decisionVariableQty] = $"f = {results[decisionVariableQty]}";


            result.Text = string.Join(Environment.NewLine, outcome);

            if (decisionVariableQty == 2)
            {
                try
                {
                    var tmp = new PlotModel(); 

                    tmp.Axes.Add(new LinearColorAxis
                    {
                        Position = AxisPosition.Right,
                        Palette = OxyPalettes.Cool(150)
                    });


                    double tmpMaxValue = Double.MinValue;
                    double x1_min = xL[0];
                    double x1_max = xU[0];
                    double x2_min = xL[1];
                    double x2_max = xU[1];
                    var x1x1 = ArrayBuilder.CreateVector(x1_min, x1_max, 100);
                    var x2x2 = ArrayBuilder.CreateVector(x2_min, x2_max, 100);
                    double[,] peaksData = new double[x1x1.GetLength(0), x2x2.GetLength(0)];
                    double[] xy_tab = new double[2];

                    for (int i = 0; i < x1x1.GetLength(0); i++)
                    {
                        for (int j = 0; j < x2x2.GetLength(0); j++)
                        {
                            xy_tab[0] = x1x1[i];
                            xy_tab[1] = x2x2[j];
                            peaksData[i, j] = function.calculate(xy_tab);
                            if (peaksData[i, j] > tmpMaxValue)
                            {
                                tmpMaxValue = peaksData[i, j];
                            }
                        }
                    }


                    var heatMapSeries = new HeatMapSeries
                    {
                        X0 = x1_min,
                        X1 = x1_max,
                        Y0 = x2_min,
                        Y1 = x2_max,
                        Interpolate = true,
                        RenderMethod = HeatMapRenderMethod.Bitmap,
                        Data = peaksData
                    };
                    tmp.Series.Add(heatMapSeries);

                    var cs = new ContourSeries
                    {
                        Color = OxyColors.BlueViolet,
                        //LabelBackground = OxyColors.Transparent, 
                        ColumnCoordinates = x1x1,
                        RowCoordinates = x2x2,
                        Data = peaksData
                    };
                    tmp.Series.Add(cs);

                    var path = new LineSeries
                    {
                        ItemsSource = ahs.GetDataPoints(),
                        Color = OxyColors.Black,
                        StrokeThickness = 1

                    };
                    tmp.Series.Add(path);


                    if (CheckParameters() == true)
                    {
                        var sc = new ScatterSeries
                        {
                            BinSize = 10,
                            MarkerType = MarkerType.Cross,
                            MarkerStrokeThickness = 3,
                          
                        };

                        double x1 = ahs.GetBestX1();
                        double x2 = ahs.GetBestX2();
                        sc.Points.Add(new ScatterPoint(x1, x2, 5, tmpMaxValue));
                        tmp.Series.Add(sc);
                    }
                    GetMainViewModel().MyModel = tmp;


                }
                catch (Exception ex)
                {

                }
            }
            CountBtn.IsEnabled = false;

        }
        public MainViewModel GetMainViewModel()
        {
            return (MainViewModel)DataContext;
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


            xL = new double[decisionVariableQty];
            for (int i = 0; i < decisionVariableQty; i++)
            {
                xL[i] = double.Parse(minTextbox[i].Text);
            }

            xU = new double[decisionVariableQty];
            for (int i = 0; i < decisionVariableQty; i++)
            {
                xU[i] = double.Parse(maxTextbox[i].Text);
            }
        }

        private void lostFocusOnObjFunc(object sender, RoutedEventArgs e)
        {
            expression = objectiveFunction.Text;

            detectedVariables.Text = string.Empty;
            decisionVariableQty = 0;

            List<string> variables = DetectVariables(expression);

            decisionVariableQty = variables.Count;

            Constraints.Visibility = Visibility.Visible;
            SetVisibleTextbox();

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
            else
            {
                MessageBox.Show(WRONG_PARAMETERS);

            }
        }

        private bool CheckMinMaxX()
        {
            for (int i = 0; i < decisionVariableQty; i++)
            {
                if (double.Parse(minTextbox[i].Text) >= double.Parse(maxTextbox[i].Text)) return false;
            }

            return true;
        }

        private bool CheckParameters()
        {
            if (double.Parse(parMax.Text) >= 0 && double.Parse(parMax.Text) <= 1
             && double.Parse(parMin.Text) >= 0 && double.Parse(parMin.Text) <= 1
             && double.Parse(parMin.Text) < double.Parse(parMax.Text)
             && double.Parse(HMCR.Text) >= 0 && double.Parse(HMCR.Text) <= 1
             && double.Parse(bwMin.Text) >= 0 && double.Parse(bwMax.Text) >= 0
             && double.Parse(bwMin.Text) < double.Parse(bwMax.Text)
             && int.Parse(Ni.Text) > 0 && int.Parse(hms.Text) > 0
             && CheckMinMaxX() && function.checkSyntax())
                return true;

            return false;
        }


    }

}

