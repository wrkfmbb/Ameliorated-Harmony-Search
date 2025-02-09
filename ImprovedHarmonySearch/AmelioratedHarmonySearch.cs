﻿using org.mariuszgromada.math.mxparser;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedHarmonySearch
{
    class AmelioratedHarmonySearch
    {

        int decisionVariableQty;
        Random rand = new Random();
        private static Function function;
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
        List<DataPoint> pathPoints = new List<DataPoint>();  //data for path 
        List<double[]> listPoints = new List<double[]>();

        public AmelioratedHarmonySearch(string variables, string expression, int decisionVariableQty, double[] xL, double[] xU, double hmcr, double parMIN, double parMAX, double bwMIN, double bwMAX, int NI, int HMS)
        {
            function = new Function($"f({variables}) = {expression}");
            this.decisionVariableQty = decisionVariableQty;
            this.xL = xL;
            this.xU = xU;
            this.hmcr = hmcr;
            this.parMIN = parMIN;
            this.parMAX = parMAX;
            this.bwMIN = bwMIN;
            this.bwMAX = bwMAX;
            this.NI = NI;
            this.HMS = HMS;

        }
        public double[] GetRowFromHM(int size)
        {
            double[] row = new double[size];  
            for (int i = 0; i < row.Length; i++)
            {
                row[i] = HM[0][i];
            }
            return row; 
        }
        public void ImprovedHarmonySearch()
        {
            int lastIterations = 10;
            int counter = NI - lastIterations;

            HM = InitializeHarmonyMemory();
            ResizeHarmonyMemoryAddingObjectiveFuncVal();
            SortByObjectiveFunctionValue(); //( O(N*log(N)) optimal O(n^2) worst)

            DataPoint dataPoint = new DataPoint(HM[0][0], HM[0][1]);
            pathPoints.Add(dataPoint);
            listPoints.Add(GetRowFromHM(decisionVariableQty+1)); 
 
            for (int i = 0; i < NI; i++)
            {
                newImprovisedHarmony = ImproviseNewHarmony(); //step 3

                fx = function.calculate(newImprovisedHarmony); //step 4 

                if (fx < HM[HMS - 1][decisionVariableQty]) // jeśli nowa wartość fx jest mniejsza od największej w posortowanje tab HM to należy dodać rozwiązanie 
                {
                    Array.Resize(ref newImprovisedHarmony, decisionVariableQty + 1);
                    newImprovisedHarmony[decisionVariableQty] = fx; // dodanie wartości funkcji celu do wektora z nowym rozwiązaniem

                    HM[HMS - 1] = newImprovisedHarmony; //wstawienie nowego wektora rozwiązań na najgorsze rozwiązanie
                    SortByObjectiveFunctionValue(); //posortuj wg wartosci funkcji celu 

                    //points for path 
                    pathPoints.Add(new DataPoint(HM[0][0], HM[0][1]));

                }
                if(i >= 0 && i < lastIterations) //10 first iterations
                {
                    listPoints.Add(GetRowFromHM(decisionVariableQty + 1));

                }
                if (i >= counter && counter < NI) //10 last iterations 
                {
                    //points from last iterations 
                    listPoints.Add(GetRowFromHM(decisionVariableQty + 1));

                }

            }
           
            listPoints.Add(GetRowFromHM(decisionVariableQty + 1));

            pathPoints.Add(new DataPoint(HM[0][0], HM[0][1]));
        }

        private double[][] InitializeHarmonyMemory()
        {
            double[][] HM = new double[HMS][];


            for (int z = 0; z < HMS; z++)
            {
                HM[z] = new double[decisionVariableQty];
            }

            for (int j = 0; j < decisionVariableQty; j++)
            {
                for (int i = 0; i < HMS; i++) //najpierw inicjalizacja dla całego x1, później dla x2, bo zakresy moga sie roznic 
                {
                    HM[i][j] = RandomNumberInScope(xL[j], xU[j]);
                }
            }

            return HM;
        }

        private double RandomNumberInScope(double lowerBound, double upperBound)
        {

            return (rand.NextDouble() * (upperBound - lowerBound)) + lowerBound;

        }

        private void ResizeHarmonyMemoryAddingObjectiveFuncVal()
        {
            double[] fV = InitializeObjectiveFuncVector();
            //powiekszenie tablicy o 1 rozmiar dla wartości  funkcji celu 
            for (int k = 0; k < HMS; k++)
            {
                Array.Resize(ref HM[k], decisionVariableQty + 1); //zwieksz rozmiar 
                HM[k][decisionVariableQty] = fV[k];               //postaw wartosc funkcji celu    
            }
        }

        private double[] InitializeObjectiveFuncVector()
        {
            double[] fValue = new double[HMS];

            for (int i = 0; i < HMS; i++)
            {
                fValue[i] = function.calculate(HM[i]);
            }

            return fValue;
        }

        private void SortByObjectiveFunctionValue()
        {
            HM = HM.OrderBy(x => x[decisionVariableQty]).ToArray();
        }

        //step 3 of algorithm 
        private double[] ImproviseNewHarmony()
        {
            double[] NHV = new double[decisionVariableQty];

            for (int i = 0; i < decisionVariableQty; i++)
            {
                PARgn = parMIN + ((parMAX - parMIN) / NI) * gn;
                c = Math.Log(bwMIN / bwMAX) / NI;
                bwgn = bwMAX * Math.Exp(c * gn);

                if (rand.NextDouble() < hmcr)
                {
                    //YES
                    D1 = (int)(rand.NextDouble() * HMS) + 1;
                    D2 = HM[D1 - 1][i];
                    NHV[i] = D2;


                    //sprawdzenie czy ulepszamy SPOSÓB NR 2
                    if (rand.NextDouble() < PARgn)
                    {
                        if (rand.NextDouble() <= 0.5)
                        {
                            //YES
                            D3 = NHV[i] - (rand.NextDouble() * bwgn);

                            if (xL[i] <= D3)
                            {
                                //YES
                                NHV[i] = D3;
                            }
                            //FOR NO DO NOTHING
                        }
                        else
                        {
                            //NO
                            D3 = NHV[i] + (rand.NextDouble() * bwgn);

                            if (xU[i] >= D3)
                            {
                                NHV[i] = D3;
                            }
                        }
                    }
                    //FOR NO DO NOTHING 

                }
                else //opcja dla 3 sposobu szukania zmiennej x[i] - randomowa wartość w zakresie xL[i]<x[i]<xU[i] 
                {
                    //NO
                    NHV[i] = RandomNumberInScope(xL[i], xU[i]);
                }

            }
            gn++;

            return NHV;
        }

        public double GetBestX1()
        {
            return HM[0][0];
        }

        public double GetBestX2()
        {
            return HM[0][1];
        }

        public List<DataPoint> GetDataPoints()
        {
            return pathPoints;
        }
        public List<double[]> GetFirstAndLastIterationResults()
        {
            return listPoints;
        }

        public string[] GetResults()
        {
            string[] result = new string[decisionVariableQty + 1];

            for (int i = 0; i <= decisionVariableQty; i++)
            {
                result[i] = HM[0][i].ToString(); // kolejność x1 x2 x3... fx  
            }

            return result;
        }
    }
}
