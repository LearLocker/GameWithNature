using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWithNature
{
    class Program
    {
        /// <summary>
        /// Метод для поиска максимального значения в массиве
        /// </summary>
        /// <param name="GMatrix">Исходный массив поиска</param>
        /// <returns>Возвращает номер максимального элемента в массиве</returns>
        static double Max(double[] matr)
        {
            double max = int.MinValue;
            int strok = 0;
            for (int i = 0; i < matr.Length; i++)
            {
                if (max < matr[i])
                {
                    max = matr[i];
                    strok = i;
                }
            }
            return strok;
        }

        /// <summary>
        /// Метод для поиска минимального значения в массиве
        /// </summary>
        /// <param name="GMatrix">Исходный массив поиска</param>
        /// <returns>Возвращает номер минимального элемента в массиве</returns>
        static double Min(double[] matr)
        {
            double min = int.MaxValue;
            int strok = 0;
            for (int i = 0; i < matr.Length; i++)
            {
                if (min > matr[i])
                {
                    min = matr[i];
                    strok = i;
                }
            }
            return strok;
        }

        /// <summary>
        /// Метод для поиска оптимального решения в игровой матрице
        /// Выбирается чистая стратегия, которая в наихудших условиях гарантирует максимальный выигрыш
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        static double Wald(List<List<double>> GMatrix)
        {
            double[] Wcriteria = new double[GMatrix.Count];

            int min = int.MaxValue;
            for(int i = 0; i < GMatrix.Count; i++)
            {
                for(int j = 0; j < GMatrix.Count; j++)
                {
                    if (min > GMatrix[i][j])
                        min = (int)GMatrix[i][j];
                }

                Wcriteria[i] = min;
                min = int.MaxValue;
            }
            return Max(Wcriteria) + 1;
        }

        /// <summary>
        /// Метод для поиска оптимального решения в игровой матрице
        /// За оптимальную принимается та стратегия, для которой выполняется соотношение:
        /// max(si)
        /// где si = y min(aij) + (1-y)max(aij)
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        static double Hurwitz(List<List<double>> GMatrix, double y)
        {
            double[] maxJ = new double[GMatrix.Count];

            for (int j = 0; j < GMatrix.Count; j++)
            {
                maxJ[j] = 0;
            }
            double[] minJ = new double[GMatrix.Count];

            for (int j = 0; j < GMatrix.Count; j++)
            {
                minJ[j] = 1000;
            }

            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                {
                    if (maxJ[i] < GMatrix[i][j])
                        maxJ[i] = GMatrix[i][j];
                }
            }

            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                {
                    if (minJ[i] > GMatrix[i][j])
                        minJ[i] = GMatrix[i][j];
                }
            }

            double[] max = new double[GMatrix.Count];

            for (int i = 0; i < GMatrix.Count; i++)
            {
                max[i] = y * minJ[i] + (1 - y) * maxJ[i];
            }

            return Max(max) + 1;
        }

        /// <summary>
        /// Метод для поиска оптимального решения в игровой матрице
        /// Выбирается стратегия, при которой величина максимального риска минимизируется в наихудших условиях
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        static double Savage(List<List<double>> GMatrix)
        {
            double[,] ma = new double[GMatrix.Count, GMatrix.Count];

            int max = 0, strok = 0;

            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                {
                    if (max < GMatrix[j][i])
                        max = (int)GMatrix[j][i];
                }
                for (int j = 0; j < GMatrix.Count; j++)

                    ma[j,i] = max - GMatrix[j][i];
                max = 0;
            }

            double[] max_str = new double[GMatrix.Count];

            max = 0;
            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                {
                    if (max < ma[i,j])
                        max = (int)ma[i,j];
                }
                max_str[i] = max;
                max = 0;
            }

            return Min(max_str) + 1;
        }

        /// <summary>
        /// Метод для поиска оптимального решения в игровой матрице
        /// Выбирается стратегия, которая при известной вероятности наступления принесет наибольший выигрыш
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        static double Ver(List<List<double>> GMatrix)
        {
            double[,] kopi_ma = new double[GMatrix.Count, GMatrix.Count];

            for (int i = 0; i < GMatrix.Count; i++)
                for (int j = 0; j < GMatrix.Count; j++)
                    kopi_ma[i,j] = GMatrix[i][j] / (double)GMatrix.Count;

            double[] max_str = new double[GMatrix.Count];

            for (int j = 0; j < GMatrix.Count; j++)
            {
                max_str[j] = 0;
            }


            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                {
                    max_str[i] += kopi_ma[i,j];
                }
            }

            return Max(max_str) + 1;
        }

        /// <summary>
        /// Точка входа в программу
        /// Программа ищет оптимальную стратегию в платежной матрице игры
        /// по критериям известных вероятностей, Вальда, Сэвиджа, Гурвица
        /// </summary>
        static void Main()
        {
            List<List<double>> GMatrix = new List<List<double>>();
            List<double> stroki = new List<double>();
            List<double> stroki2 = new List<double>();
            List<double> stroki3 = new List<double>();
            stroki.Add(0); stroki.Add(2); stroki.Add(5);
            GMatrix.Add(stroki);
            stroki2.Add(2); stroki2.Add(3); stroki2.Add(1);
            GMatrix.Add(stroki2);
            stroki3.Add(4); stroki3.Add(3); stroki3.Add(-1);
            GMatrix.Add(stroki3);

            for (int i = 0; i < GMatrix.Count; i++)
            {
                for (int j = 0; j < GMatrix.Count; j++)
                    Console.Write(" {0}", GMatrix[i][j]);
                Console.WriteLine();
            }

            int wald = (int)Wald(GMatrix);
            Console.WriteLine("По критерию Вальда вам нужно выбрать стратегию № {0}", wald);
            for (int j = 0; j < GMatrix.Count; j++)
                Console.Write(" {0}", GMatrix[wald-1][j]);
            Console.WriteLine();
            int Sevidj = (int)Savage(GMatrix);

            Console.WriteLine("По критерию Сэвиджа вам нужно выбрать стратегию № {0}", Sevidj);
            for (int j = 0; j < GMatrix.Count; j++)
                Console.Write(" {0}", GMatrix[Sevidj-1][j]);
            Console.WriteLine();
            int Gurvic = (int)Hurwitz(GMatrix, 0.5);

            Console.WriteLine("По критерию Гурвица вам нужно выбрать стратегию № {0}", Gurvic);
            for (int j = 0; j < GMatrix.Count; j++)
                Console.Write(" {0}", GMatrix[Gurvic-1][j]);
            Console.WriteLine();
           int Baies = (int)Ver(GMatrix);

            Console.WriteLine("По критерию известных вероятностей вам нужно выбрать стратегию № {0}", Baies);
            for (int j = 0; j < GMatrix.Count; j++)
                Console.Write(" {0}", GMatrix[Baies-1][j]);
            Console.WriteLine();

            Console.ReadKey();
        }

    }
}
