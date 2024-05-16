using Plotly.NET;
using MathNet.Numerics.LinearAlgebra;
using System.Runtime.CompilerServices;

namespace Tims
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            int [] x = {2, 3, 5, 7, 9, 12, 13};
            int [] y = {3, 4, 5, 6, 10, 12};
            int [,] table = {
                {0,0,0,0,0,21,1},
                {0,0,2,3,20,0,0},
                {0,2,31,12,4,0,0},
                {0,15,3,0,0,0,0},
                {3,7,0,0,0,0,0},
                {25,0,0,0,0,0,0}
            };
            int [] sumax_n = new int [x.Length];
            for(int i = 0; i < x.Length; i++)
            {
                int suma_x = 0;
                for(int j = 0; j < y.Length; j++)
                {
                    suma_x += table[j,i];
                }
                sumax_n[i] = suma_x;
                Console.WriteLine($"Sum of xn{i+1}: {sumax_n[i]}");
            }

            Console.Write("\nCalculating conditional averages");
            Console.Write("\nx");

            for(int i = 0; i <x.Length; i++)
            {
                if(i == x.Length -1)
                {
                    Console.WriteLine($"\tx{i+1}");
                }
                else
                {
                    Console.Write($"\tx{i+1}");
                }
            }
            
            Console.Write("y_avgs");

            double [] uy = new double[x.Length];

            for(int i = 0; i < x.Length; i++)
            {
                
                for(int j =0; j < y.Length; j++)
                {
                    uy[i] += y[j] * table[j,i];
                }
                uy[i] = Math.Round(uy[i] / Convert.ToDouble(sumax_n[i]), 3);
                
                Console.Write($"\t{uy[i]}");
            }
            var points = Chart2D.Chart.Point<int, double, double>(x, uy, Name: "M(x_i, y_ser_i)");
            points.Show();
           
            double [] systemsOfEquations = new double[5];

            for(int i = 0; i < x.Length; i++)
            {
              systemsOfEquations[0] += sumax_n[i] / x[i];
              systemsOfEquations[1] += sumax_n[i];
              systemsOfEquations[2] += uy[i] * sumax_n[i];
              systemsOfEquations[3] += sumax_n[i] / Math.Pow(x[i], 2);
              systemsOfEquations[4] += uy[i] * sumax_n[i] / x[i];
            }

            var A = Matrix<double>.Build.DenseOfArray(new double[,]{
                {systemsOfEquations[0], systemsOfEquations[1]},
                {systemsOfEquations[3], systemsOfEquations[0]}
            });

            var B = Vector<double>.Build.Dense(new double[]{systemsOfEquations[2], systemsOfEquations[4]});
            var X = A.Solve(B);
            double a = Math.Round(X[0],3);
            double b = Math.Round(X[1],3);

            
            Console.WriteLine($"\n\na = {a}");
            Console.WriteLine($"b = {b}");

            Console.WriteLine("Equations for hyperbole is: ");
            Console.WriteLine($"y_ser = {a}/x + {b}\n");

            var points1 = Chart2D.Chart.Point<int, double, double>(x, uy, Name: "M(x_i, y_ser_i)");
            
            double[] xRegres = new double[1000];
            double[] yRegres = new double[1000];
            xRegres[0] = 1;
            yRegres[0] = (a/xRegres[0]) + b;
            for(int i = 1; i < 1000; i++)
            {
                xRegres[i] = xRegres[i-1] + 0.013;
                yRegres[i] = a/xRegres[i] + b;
            }

            var regres = Chart2D.Chart.Line<double, double, double>(x: xRegres, y: yRegres, Name: "y_ser = a/x + b");
            regres.Show();
            
            var chart = Chart.Combine(new [] {points1, regres});
            chart.Show();

            double sigma = 0;

            for(int i = 0; i < x.Length; i++)
            {
                for(int j = 0; j < y.Length; j++)
                {
                    sigma += Math.Pow(y[j] - (a/x[j] + b), 2) * table[j,i];
                }
            }

            sigma /= sumax_n.Sum();
            sigma = Math.Round(sigma, 3);
            Console.WriteLine($"\nSigma = {sigma}");

            double skv = 0;

            for(int i = 0; i < x.Length; i++)
            {
              skv += Math.Pow(uy[i] - (a/x[i] + b), 2) * sumax_n[i];
            }
            skv = Math.Round(skv, 3);
            Console.WriteLine($"\nSkv = {skv}");

        }
    }
}
