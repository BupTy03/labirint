using System;
using System.Collections.Generic;

namespace Labirint_prog
{
    class Matrix
    {
        protected struct cell
        {
            public int y;   // координаты в print_arr
            public int x;
            public bool use;
            public int num;
            // наличие/отсутствие границ
            public bool right_w;    // справа
            public bool left_w;     // слева
            public bool ceiling;    // сверху
            public bool floor;      // снизу
        }

        protected int height;    // высота
        protected int width;    // ширина
        protected int h_of_cell;    // высота клетки
        protected int w_of_cell;    // ширина клетки
        protected int print_h;    // высота массива "для печати"
        protected int print_w;    // ширина массива "для печати"
        protected int maxnum;     // максимальный индекс элемента
        protected cell[,] arr_of_c;   // сама матрица
        protected bool[,] print_arr;  // массив "для печати"

        public Matrix(int height, int width, int h_of_cell = 2, int w_of_cell = 5)
        {
            if (height <= 0 || width <= 0 || h_of_cell <= 0 || w_of_cell <= 0)
                throw new Exception("Параметры конструктора класса Matrix должны быть больше нуля.");

            this.height = height;
            this.width = width;
            this.h_of_cell = h_of_cell;
            this.w_of_cell = w_of_cell;

            print_h = height * h_of_cell + 1;
            print_w = width * w_of_cell + 1;
            maxnum = height * width - 1;
            arr_of_c = new cell[height, width];
            print_arr = new bool[print_h, print_w];
            init_cells();

            for (int i = 0; i < print_h; i++)
            {   // границы и сетка
                for (int j = 0; j < print_w; j++)
                {
                    if (i % h_of_cell == 0 || j % w_of_cell == 0) // сетка 
                        print_arr[i, j] = true;
                    else
                        print_arr[i, j] = false;
                }
            }
        }

        protected void init_cells() // инициализирует ячейки
        {
            int n = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {

                    if (i == 0)
                        arr_of_c[i, j].ceiling = false;
                    else
                        arr_of_c[i, j].ceiling = true;

                    if (j == 0)
                        arr_of_c[i, j].left_w = false;
                    else
                        arr_of_c[i, j].left_w = true;

                    if (i == height - 1)
                        arr_of_c[i, j].floor = false;
                    else
                        arr_of_c[i, j].floor = true;

                    if (j == width - 1)
                        arr_of_c[i, j].right_w = false;
                    else
                        arr_of_c[i, j].right_w = true;

                    arr_of_c[i, j].use = false;

                    arr_of_c[i, j].y = i * h_of_cell;   // высота ячейки в сетке
                    arr_of_c[i, j].x = j * w_of_cell;   // ширина ячейки в сетке

                    arr_of_c[i, j].num = n;
                    n++;
                }
        }
        public void Use_cell(int n) // отмечает использованные ячейки
        {
            if (n < 0)
                throw new Exception("Функции Use_cell() передан параметр меньший нуля.");
            arr_of_c[n / width, n % width].use = true;
        }
        public void Use_cell(int i, int j)
        {
            if (i < 0 || j < 0)
                throw new Exception("Функции Use_cell() передан параметр меньший нуля.");
            arr_of_c[i, j].use = true;
        }
        public bool IsUsed(int n)   // проверяет не использована ли ячейка
        {
            if (n < 0)
                throw new Exception("Функции IsUsed() передан параметр меньший нуля.");
            return arr_of_c[n / width, n % width].use;
        }
        public bool IsUsed(int i, int j)
        {
            if (i < 0 || j < 0)
                throw new Exception("Функции IsUsed() передан параметр меньший нуля.");
            return arr_of_c[i, j].use;
        }
        public bool Unused_cells()  // проверяет остались ли неиспользованные ячейки
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (!arr_of_c[i, j].use)
                        return true;

            return false;
        }
        public bool Unused_neighbors(int n)  // проверяет остались ли неиспользованные ячейки по соседству
        {
            if (n < 0)
                throw new Exception("Функции Unused_neighbors() передан параметр меньший нуля.");
            bool flag = false;
            int i = n / width;
            int j = n % width;
            if (arr_of_c[i, j].left_w && !arr_of_c[i, j - 1].use)
                flag = true;
            if (arr_of_c[i, j].right_w && !arr_of_c[i, j + 1].use)
                flag = true;
            if (arr_of_c[i, j].ceiling && !arr_of_c[i - 1, j].use)
                flag = true;
            if (arr_of_c[i, j].floor && !arr_of_c[i + 1, j].use)
                flag = true;

            return flag;
        }
        public bool Unused_neighbors(int i, int j)
        {
            if (i < 0 || j < 0)
                throw new Exception("Функции Unused_neighbors() передан параметр меньший нуля.");
            bool flag = false;

            if (arr_of_c[i, j].left_w && !arr_of_c[i, j - 1].use)
                flag = true;
            if (arr_of_c[i, j].right_w && !arr_of_c[i, j + 1].use)
                flag = true;
            if (arr_of_c[i, j].ceiling && !arr_of_c[i - 1, j].use)
                flag = true;
            if (arr_of_c[i, j].floor && !arr_of_c[i + 1, j].use)
                flag = true;

            return flag;
        }

        public void Print()
        {
            for (int i = 0; i < print_h; i++)
            {
                for (int j = 0; j < print_w; j++)
                {
                    if (print_arr[i, j])
                        Console.Write('#');
                    else Console.Write(' ');
                }
                Console.WriteLine();
            }
        }
    }

    class Labirint : Matrix
    {
        public Labirint(int height, int width, int h_of_cell = 2, int w_of_cell = 5) : base(height, width, h_of_cell, w_of_cell)
        {
        }

        public void delWall(int n1, int n2) // удаляет "стену" между ячейками
        {
            if (n1 < 0 || n2 < 0 || n1 > maxnum || n2 > maxnum)
                throw new Exception("Функции delWall() передан(ы) неверный(е) параметр(ы).");

            int i1 = n1 / width;
            int j1 = n1 % width;
            int i2 = n2 / width;
            int j2 = n2 % width;

            if (i2 - i1 < 0 && j1 == j2)
                Hole(n1, 'c');  // прорубить ход от первой ячейки вверх
            else if (i2 - i1 > 0 && j1 == j2)
                Hole(n1, 'f');  // прорубить ход от первой ячейки вниз
            else if (j2 - j1 < 0 && i1 == i2)
                Hole(n1, 'l');  // прорубить ход от первой ячейки влево
            else if (j2 - j1 > 0 && i1 == i2)
                Hole(n1, 'r');  // прорубить ход от первой ячейки вправо
        }

        public int Next_step(int curr_num)
        {
            if (curr_num < 0 || width <= 0)
                throw new Exception("Функции Next_step() передан(ы) неверный(е) параметр(ы).");
            int i = curr_num / width;
            int j = curr_num % width;

            Random rand = new Random(Convert.ToInt32(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));

            int next = 0;
            bool k = true;
            while (k)
            {
                int new_i = rand.Next(height), new_j = rand.Next(width);
                if (new_i != i && new_j == j && new_i - i == 1 && arr_of_c[i, j].floor && !IsUsed(new_i, new_j))
                {
                    next = new_i * width + j;
                    k = false;
                }
                else if (new_i != i && new_j == j && i - new_i == 1 && arr_of_c[i, j].ceiling && !IsUsed(new_i, new_j))
                {
                    next = new_i * width + j;
                    k = false;
                }
                else if (new_j != j && new_i == i && new_j - j == 1 && arr_of_c[i, j].right_w && !IsUsed(new_i, new_j))
                {
                    next = i * width + new_j;
                    k = false;
                }
                else if (new_j != j && new_i == i && j - new_j == 1 && arr_of_c[i, j].left_w && !IsUsed(new_i, new_j))
                {
                    next = i * width + new_j;
                    k = false;
                }
            }
            return next;
        }   // выбирает следующую соседнюю ячейку

        public void Hole(int num, char wall)   // "прорубает" вход и выход
        {
            int i = num / width;
            int j = num % width;

            switch (wall)
            {
                case 'c':
                    for (int k = 1; k < w_of_cell; k++)
                        print_arr[arr_of_c[i, j].y, arr_of_c[i, j].x + k] = false;
                    break;
                case 'f':
                    for (int k = 1; k < w_of_cell; k++)
                        print_arr[arr_of_c[i, j].y + h_of_cell, arr_of_c[i, j].x + k] = false;
                    break;
                case 'l':
                    for (int k = 1; k < h_of_cell; k++)
                        print_arr[arr_of_c[i, j].y + k, arr_of_c[i, j].x] = false;
                    break;
                case 'r':
                    for (int k = 1; k < h_of_cell; k++)
                        print_arr[arr_of_c[i, j].y + k, arr_of_c[i, j].x + w_of_cell] = false;
                    break;
            }
        }
    }

    class MainClass
    {

        public static void Main(string[] args)
        {

            try
            {
                int height = 0, width = 0;
                while (height <= 0 || width <= 0)
                {
                    Console.Write("Введите высоту лабиринта(целое положительное число)\n> ");
                    height = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Введите ширину лабиринта(целое положительное число)\n> ");
                    width = Convert.ToInt32(Console.ReadLine());
                    if (height <= 0 || width <= 0)
                        Console.Write("Пожалуйста, повторите ввод:\n");
                }
                Labirint lab = new Labirint(height, width, 2, 4);
                Random run = new Random(Convert.ToInt32(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));

                int current = 0; // элемент с которого нужно начать
                lab.Use_cell(current);
                lab.Hole(current, 'l');    // вход в лабиринт (номер ячейки; правая, левая стена, пол 
                //или потолок)

                Stack<int> stack_of_cells = new Stack<int>();
                stack_of_cells.Push(height * width - 1);
                lab.Hole(height * width - 1, 'r');

                while (lab.Unused_cells())
                {
                    if (lab.Unused_neighbors(current))
                    {
                        stack_of_cells.Push(current);
                        int next = lab.Next_step(current);
                        lab.delWall(current, next);
                        current = next;
                        lab.Use_cell(current);
                    }
                    else if (stack_of_cells.Count > 0)
                    {
                        current = stack_of_cells.Pop();
                        lab.Use_cell(current);
                    }
                    else
                    {
                        if (lab.Unused_cells())
                        {
                            while (true)
                            {
                                Console.WriteLine('E');
                                current = run.Next(height * width);
                                if (!lab.IsUsed(current))
                                {
                                    lab.Use_cell(current);
                                    break;
                                }
                            }
                        }
                        else break;
                    }
                }

                lab.Print();
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Вы ввели пустую строку(удивительно, как вы так умудрились?) :)");
            }
            catch (System.FormatException)
            {
                System.Console.WriteLine("Ошибка формата ввода :(");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}