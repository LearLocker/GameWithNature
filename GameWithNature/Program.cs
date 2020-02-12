using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GameWithNature
{
    public class Game
    {
        private double[,] GMatrix;
        private double[,] RMatrix;
        private int rows;
        private int columns;
        private int Rows { get => rows; set => rows = value; }
        private int Columns { get => columns; set => columns = value; }
        public Game()
        {
            // Считывание данных из файла
            string[] lines = File.ReadAllLines("input.txt");
            GMatrix = new double[lines.Length, lines[0].Split(' ').Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(' ');
                for (int j = 0; j < temp.Length; j++)
                    GMatrix[i, j] = Convert.ToDouble(temp[j]);
            }

            Rows = GMatrix.GetUpperBound(0) + 1;
            Columns = GMatrix.Length / rows;
            // или так
            // int columns = mas.GetUpperBound(1) + 1;

            RiskMatrix();
        }

        public double[,] GetMatrix()
        {
            if (GMatrix != null)
                return GMatrix;
            else
                throw new Exception("Matrix does not exist");
        }

        /// <summary>
        /// Метод для поиска максимального значения в массиве
        /// </summary>
        /// <param name="GMatrix">Исходный массив поиска</param>
        /// <returns>Возвращает номер максимального элемента в массиве</returns>
        public int Max(double[] matr)
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
        public int Min(double[] matr)
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
        /// Метод Вальда для поиска оптимального решения в игровой матрице
        /// Выбирается чистая стратегия, которая в наихудших условиях гарантирует максимальный выигрыш
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        public int Wald()
        {
            double[] Wcriteria = new double[Rows];

            int min = int.MaxValue;
            for (int i = 0; i < GMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < GMatrix.GetLength(1); j++)
                {
                    if (min > GMatrix[i, j])
                        min = (int)GMatrix[i, j];
                }

                Wcriteria[i] = min;
                min = int.MaxValue;
            }
            return Max(Wcriteria) + 1;
        }

        /// <summary>
        /// Метод Гурвица для поиска оптимального решения в игровой матрице
        /// За оптимальную принимается та стратегия, для которой выполняется соотношение:
        /// max(si)
        /// где si = y min(aij) + (1-y)max(aij)
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        public int Hurwitz(double y)
        {
            double[] maxJ = new double[GMatrix.GetLength(0)];

            for (int j = 0; j < GMatrix.GetLength(0); j++)
            {
                maxJ[j] = 0;
            }
            double[] minJ = new double[GMatrix.GetLength(0)];

            for (int j = 0; j < GMatrix.GetLength(0); j++)
            {
                minJ[j] = 1000;
            }

            for (int j = 0; j < GMatrix.GetLength(0); j++)
            { 
                for (int i = 0; i < GMatrix.GetLength(1); i++)
                {
                    if (maxJ[j] < GMatrix[j, i])
                        maxJ[j] = GMatrix[j, i];
                }
            }

            for (int j = 0; j < GMatrix.GetLength(0); j++)
            {
                for (int i = 0; i < GMatrix.GetLength(1); i++)
                {
                    if (minJ[j] > GMatrix[j, i])
                        minJ[j] = GMatrix[j, i];
                }
            }

            double[] max = new double[GMatrix.GetLength(0)];

            for (int i = 0; i < GMatrix.GetLength(0); i++)
            {
                max[i] = y * minJ[i] + (1 - y) * maxJ[i];
            }

            return Max(max) + 1;
        }

        /// <summary>
        /// Метод Сэвиджа для поиска оптимального решения в игровой матрице
        /// Выбирается стратегия, при которой величина максимального риска минимизируется в наихудших условиях
        /// </summary>
        /// <param name="RMatrix">Исходная игровая матрица</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        public int Savage()
        {
            //double[,] ma = new double[GMatrix.Count, GMatrix.Count];

            int max = 0;
            double[] SCriteria = new double[RMatrix.GetLength(0)];

            for (int i = 0; i < RMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < RMatrix.GetLength(1); j++)
                {
                    if (max < RMatrix[i, j])
                        max = (int)RMatrix[i, j];
                }
                SCriteria[i] = max;
                max = 0;
            }

            return Min(SCriteria) + 1;
        }

        /// <summary>
        /// Метод для поиска оптимального решения в игровой матрице
        /// Выбирается стратегия, которая при известной вероятности наступления принесет наибольший выигрыш
        /// </summary>
        /// <param name="GMatrix">Исходная игровая матрица</param>
        /// <param name="Q">Матрица вероятностей наступления условий Пj</param>
        /// <returns>Возвращает номер оптимального критерия</returns>
        public int Ver(double[] Q)
        {
            double[] kopi_ma = new double[GMatrix.GetLength(0)];

            for (int i = 0; i < GMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < GMatrix.GetLength(1); j++)
                    kopi_ma[i] += Q[j] * GMatrix[i, j];
            }

            return Max(kopi_ma) + 1;
        }

        /// <summary>
        /// Метод для вычисления матрицы рисков
        /// </summary>
        /// <param name="GMatrix">Исходная матрица</param>
        /// <returns>Возвращает матрицу рисков</returns>
        public void RiskMatrix()
        {
            RMatrix = new double[GMatrix.GetLength(0), GMatrix.GetLength(1)];

            int max = int.MinValue;
            double[] max_el = new double[GMatrix.GetLength(1)];

            for (int j = 0; j < GMatrix.GetLength(1); j++)
            {
                for (int i = 0; i < GMatrix.GetLength(0); i++)
                {
                    if (max < GMatrix[i, j])
                        max = (int)GMatrix[i, j];
                }

                max_el[j] = max;
                max = 0;
            }
            for (int j = 0; j < GMatrix.GetLength(1); j++)
                for (int i = 0; i < GMatrix.GetLength(0); i++)
                    RMatrix[i, j] = max_el[j] - GMatrix[i, j];
        }

        public double[,] GetRiskMatrix()
        {
            return RMatrix;
        }

        /// <summary>
        /// Метод для вывода основной игровой матрицы
        /// </summary>
        /// <param name="GMatrix">Исходная матрица</param>
        public void OutputGMatrix()
        {
            Console.WriteLine("Исходная матрица");

            for (int i = 0; i < GMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < GMatrix.GetLength(1); j++)
                    Console.Write(" {0}", GMatrix[i, j]);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Метод для вывода матрицы рисков
        /// </summary>
        /// <param name=RGMatrix">Матрица рисков</param>
        public void OutputRMatrix()
        {
            Console.WriteLine("Матрица рисков");

            for (int i = 0; i < RMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < RMatrix.GetLength(1); j++)
                    Console.Write(" {0}", RMatrix[i, j]);
                Console.WriteLine();
            }
        }
    }
    class Program
    {
        /// <summary>
        /// Точка входа в программу
        /// Программа ищет оптимальную стратегию в платежной матрице игры
        /// по критериям известных вероятностей, Вальда, Сэвиджа, Гурвица
        /// </summary>
        static void Main()
        {
            Game game = new Game();
            game.OutputGMatrix();
            game.OutputRMatrix();

            double[,] gameMatrix = game.GetMatrix();


            // Поиск оптимальной стратегии по заданным критериям с выводом на экран
            int wald = game.Wald();

            Console.WriteLine("По критерию Вальда вам нужно выбрать стратегию № {0}", wald);
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
                Console.Write(" {0}", gameMatrix[wald - 1, j]);
            Console.WriteLine();

            int Sevidj = game.Savage();
            Console.WriteLine("По критерию Сэвиджа вам нужно выбрать стратегию № {0}", Sevidj);
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
                Console.Write(" {0}", gameMatrix[Sevidj - 1, j]);
            Console.WriteLine();

            string userString = "";
            Console.WriteLine("Введите критерий Гурвица");
            double GRVZ = Convert.ToDouble(Console.ReadLine());

            int Gurvic = game.Hurwitz(GRVZ);
            Console.WriteLine("По критерию Гурвица вам нужно выбрать стратегию № {0}", Gurvic);
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
                Console.Write(" {0}", gameMatrix[Gurvic - 1, j]);
            Console.WriteLine();

            userString = "";
            Console.WriteLine("Введите вероятности каждого условия({0})", gameMatrix.GetLength(1));
            double[] Q = new double[gameMatrix.GetLength(1)];
            userString = Console.ReadLine();
            string[] UserStr = userString.Split(' ');
            for (int j = 0; j < UserStr.Length; j++)
                Q[j] = Convert.ToDouble(UserStr[j]);

            int Baies = game.Ver(Q);
            Console.WriteLine("По критерию известных вероятностей вам нужно выбрать стратегию № {0}", Baies);
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
                Console.Write(" {0}", gameMatrix[Baies - 1, j]);
            Console.WriteLine();

            Console.ReadKey();

        }

    }
}
