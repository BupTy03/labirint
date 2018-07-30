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
            public bool use;	// использована ли ячейка
            public int num;     // порядковый номер ячейки

            // наличие/отсутствие границ
            public bool right_w;    // справа
            public bool left_w;     // слева
            public bool ceiling;    // сверху
            public bool floor;      // снизу
        }

        protected const int max_cells_count = 1000; // максимальное число ячеек
        protected const int min_cells_count = 1;    // минимальное число ячеек
        protected const int max_cell_size = 100;    // максимальный размер ячейки
        protected const int min_cell_size = 1;      // минимальный размер ячейки

        protected int height;    		// высота
        protected int width;    		// ширина
        protected int h_of_cell;    	// высота клетки
        protected int w_of_cell;    	// ширина клетки
        protected int print_h;    		// высота массива "для печати"
        protected int print_w;    		// ширина массива "для печати"
        protected int maxnum;     		// максимальный индекс элемента
        protected cell[,] arr_of_c;   	// сама матрица
        protected bool[,] print_arr;  	// массив "для печати"

        protected void error(string name)
        {
            throw new Exception(name);
        }

        protected void check_range(int x, int lo, int hi)
        {
            if (x < lo || x > hi)
                throw new ArgumentOutOfRangeException();
        }

        public Matrix(int height, int width, int h_of_cell = 2, int w_of_cell = 5)
        {
            check_range(height, min_cells_count, max_cells_count);
            check_range(width, min_cells_count, max_cells_count);
            check_range(h_of_cell, min_cell_size, max_cell_size);
            check_range(w_of_cell, min_cell_size, max_cell_size);

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

            for (int i = 0; i < print_h; i++)   // границы и сетка
                for (int j = 0; j < print_w; j++)
                    print_arr[i, j] = (i % h_of_cell == 0 || j % w_of_cell == 0);
        }

        protected void init_cells() // инициализирует ячейки
        {
            int n = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {                    
                    arr_of_c[i, j].ceiling  = (i != 0);
                    arr_of_c[i, j].left_w   = (j != 0);
                    arr_of_c[i, j].floor    = (i != height - 1);
                    arr_of_c[i, j].right_w  = (j != width - 1);

                    arr_of_c[i, j].use = false;

                    arr_of_c[i, j].y = i * h_of_cell;   // высота ячейки в сетке
                    arr_of_c[i, j].x = j * w_of_cell;   // ширина ячейки в сетке

                    arr_of_c[i, j].num = n;
                    n++;
                }
            }
        }
        public void Use_cell(int n) // отмечает использованные ячейки
        {
            check_range(n, 0, maxnum);
            arr_of_c[n / width, n % width].use = true;
        }
        public bool IsUsed(int i, int j)    // проверяет не использована ли ячейка
        {
            check_range(i, 0, height - 1);
            check_range(j, 0, width - 1);
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
            check_range(n, 0, maxnum);
            int i = n / width;
            int j = n % width;

            return arr_of_c[i, j].left_w && !arr_of_c[i, j - 1].use ||
                    arr_of_c[i, j].right_w && !arr_of_c[i, j + 1].use ||
                    arr_of_c[i, j].ceiling && !arr_of_c[i - 1, j].use ||
                    arr_of_c[i, j].floor && !arr_of_c[i + 1, j].use;
        }

        public void Print()	// "печатает" результат в консоль
        {
            for (int i = 0; i < print_h; i++)
            {
                for (int j = 0; j < print_w; j++)
                    Console.Write((print_arr[i, j]) ? '#' : ' ');

                Console.WriteLine();
            }
        }
    }

    class Labirint : Matrix
    {
        public Labirint(int height, int width, int h_of_cell = 2, int w_of_cell = 5) : base(height, width, h_of_cell, w_of_cell)
        {
        }

        public void delWall(int n1, int n2) // удаляет "стену" между двумя ячейками
        {
            check_range(n1, 0, maxnum);
            check_range(n2, 0, maxnum);

            int i1 = n1 / width;
            int j1 = n1 % width;
            int i2 = n2 / width;
            int j2 = n2 % width;

            if (i2 - i1 < 0 && j1 == j2)
                Hole(n1, 'c');  // прорубить ход от первой ячейки ко второй вверх
            else if (i2 - i1 > 0 && j1 == j2)
                Hole(n1, 'f');  // прорубить ход от первой ячейки ко второй вниз
            else if (j2 - j1 < 0 && i1 == i2)
                Hole(n1, 'l');  // прорубить ход от первой ячейки ко второй влево
            else if (j2 - j1 > 0 && i1 == i2)
                Hole(n1, 'r');  // прорубить ход от первой ячейки ко второй вправо
        }

        public int Next_step(int curr_num)  // выбирает следующую соседнюю ячейку
        {
            check_range(curr_num, 0, maxnum);

            int i = curr_num / width;
            int j = curr_num % width;

            Random rand = new Random(Convert.ToInt32(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));

            int next = 0;
            while (true)
            {
                int new_i = rand.Next(height);
                int new_j = rand.Next(width);
                if (new_i != i && new_j == j && new_i - i == 1 && arr_of_c[i, j].floor && !IsUsed(new_i, new_j))
                {
                    next = new_i * width + j;
                    break;
                }
                else if (new_i != i && new_j == j && i - new_i == 1 && arr_of_c[i, j].ceiling && !IsUsed(new_i, new_j))
                {
                    next = new_i * width + j;
                    break;
                }
                else if (new_j != j && new_i == i && new_j - j == 1 && arr_of_c[i, j].right_w && !IsUsed(new_i, new_j))
                {
                    next = i * width + new_j;
                    break;
                }
                else if (new_j != j && new_i == i && j - new_j == 1 && arr_of_c[i, j].left_w && !IsUsed(new_i, new_j))
                {
                    next = i * width + new_j;
                    break;
                }
            }
            return next;
        }

        public void Hole(int num, char wall)   // "прорубает" вход и выход
        {
            check_range(num, 0, maxnum);

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
                default:
                    error("Неверный формат: ожидалось 'c', 'f', 'l' или 'r'");
                    break;
            }
        }
    }

    class MainClass
    {
        public static bool isInRange(int x, int lo, int hi) // проверяет находится ли x в диапазоне lo hi
        {
            return x >= lo && x <= hi;
        }

        public static void Main(string[] args)
        {

            try
            {
                const int max_high = 1000;
                const int max_width = 1000;
                const int min_high = 1;
                const int min_width = 1;

                int height = 0, width = 0;
                while (!isInRange(height, min_high, max_high) || !isInRange(width, min_width, max_width))
                {
                    Console.Write("Введите высоту лабиринта(целое положительное число)\n> ");
                    height = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Введите ширину лабиринта(целое положительное число)\n> ");
                    width = Convert.ToInt32(Console.ReadLine());
                    if (!isInRange(height, min_high, max_high) || !isInRange(width, min_width, max_width))
                        Console.Write("Пожалуйста, повторите ввод:\n");
                }
                Labirint lab = new Labirint(height, width, 2, 4);
                Random run = new Random(Convert.ToInt32(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));

                int current = 0;            // элемент с которого нужно начать
                lab.Use_cell(current);
                lab.Hole(current, 'l');     // вход в лабиринт 
                                            //(номер ячейки; правая('r'), левая('l') стена, пол('f') 
                                            //или потолок('c'))

                Stack<int> stack_of_cells = new Stack<int>();   // стек ячеек
                stack_of_cells.Push(height * width - 1);        // добавляем последнюю ячейку в стек(для выхода)
                lab.Hole(height * width - 1, 'r');              // прорубаем выход

                while (lab.Unused_cells())  // пока остались неиспользованные ячейки
                {
                    if (lab.Unused_neighbors(current))  // если у текущей ячейки остались неиспользованные соседи
                    {
                        stack_of_cells.Push(current);       // добавляем текущую ячейку в стек
                        int next = lab.Next_step(current);  // вычисляем номер следующей ячейки
                        lab.delWall(current, next);         // удаляем "стену" между текущей и следующей ячейкой
                        current = next;                     // следующая ячейка становится текущей
                        lab.Use_cell(current);              // помечаем следующую ячейку как использованную
                    }
                    else if (stack_of_cells.Count > 0)      // если неиспользованных соседей нет
                    {                                       // и в стеке ещё остались ячейки
                        current = stack_of_cells.Pop();     // ячейка из стека становится текущей
                        lab.Use_cell(current);              // помечаем как использованную
                    }
                }

                lab.Print();
            }
            catch (System.ArgumentOutOfRangeException)
            {
                System.Console.WriteLine("Выход за пределы диапазона");
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Вы ввели пустую строку(удивительно, как вы так умудрились?) :)");
            }
            catch (System.FormatException)
            {
                System.Console.WriteLine("Ошибка: неправильный ввод :(");
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