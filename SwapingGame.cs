/**************************************************************************
 * SwappingGame.cpp
 * A similar game to candy crush but in a terminal
 * 
 * Created by ASM-GP.
 * 2024-3-12
 * 1.1.2
 * 
 * Last updated on 2024-3-19 
 **************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Monkey
{
    internal class SwapingGame
    {
        private readonly SGrid g;
        Pos P;
        ConsoleKeyInfo Control { get; set; }
        private readonly ConsoleColor[] Color = new ConsoleColor[]
        {
            ConsoleColor.Black,
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Blue,
            ConsoleColor.Magenta
        };
        bool Hold;
        int Score, Falling;
        const int Fall_Speed = 3;
        public SwapingGame() 
        {
            g = new SGrid(30,20);
            P = new Pos(5,5);
            Console.CursorVisible = false;
            Falling = 0;
        }
        public void MainSetting() 
        {
            while (true) 
            {
                Input();

                SHowGridBlocks();

                Falling++;

                if (Falling is Fall_Speed) 
                {
                    Falling = 0;

                    if (!g.Drop()) 
                    {
                        g.CheckEntireGrid();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(P.row, P.column);
                Console.Write('x');

                Console.SetCursorPosition(34,2);
                Console.Write("Score: {0}     Time: {1} ", Score, Falling);

                Thread.Sleep(70);
            }
        }
        void Input() 
        {
            if (Console.KeyAvailable)
            {
                Control = Console.ReadKey(true);

                if (Control.Key is ConsoleKey.Spacebar)
                {
                    Hold = true;
                }
                else if (!Hold)
                {
                    if (Control.Key is ConsoleKey.RightArrow && P.row+1 < g.row)
                    {
                        Move(1,0);
                    }
                    else if (Control.Key is ConsoleKey.LeftArrow && P.row-1 >= 0)
                    {
                        Move(-1,0);
                    }
                    else if (Control.Key is ConsoleKey.UpArrow && P.column - 1 >= 0)
                    {
                        Move(0,-1);
                    }
                    else if (Control.Key is ConsoleKey.DownArrow && P.column+1 < g.column)
                    {
                        Move(0,1);
                    }
                }
                else
                {
                    if (Control.Key is ConsoleKey.RightArrow && g.InsideGrid(P.row + 1, P.column))
                    {
                        SwapLeftRight(P.row, P.column);

                        if ((!g.CheckInside(P.row, P.column) && !g.CheckInside(P.row + 1, P.column)) ||
                            g.IsFallingState(P.row, P.column) || g.IsFallingState(P.row+1, P.column))
                        {
                            SwapLeftRight(P.row, P.column);
                        }

                        if (g.CheckInside(P.row, P.column))
                        {
                            Score += g.Clear(P.row, P.column);
                        }

                        if (g.CheckInside(P.row + 1, P.column)) 
                        {
                            Score += g.Clear(P.row+1, P.column);
                        }

                        Hold = false;
                    }
                    else if (Control.Key is ConsoleKey.LeftArrow && g.InsideGrid(P.row-1, P.column))
                    {
                        SwapLeftRight(P.row-1, P.column);

                        if ((!g.CheckInside(P.row, P.column) && !g.CheckInside(P.row-1, P.column)) ||
                            g.IsFallingState(P.row, P.column) || g.IsFallingState(P.row - 1, P.column))
                        {
                            SwapLeftRight(P.row - 1, P.column);
                        }

                        if (g.CheckInside(P.row, P.column))
                        {
                            Score += g.Clear(P.row, P.column);
                        }

                        if (g.CheckInside(P.row - 1, P.column)) 
                        {
                            Score += g.Clear(P.row-1, P.column);
                        }

                        Hold = false;
                    }
                    else if (Control.Key is ConsoleKey.UpArrow && g.InsideGrid(P.row, P.column-1))
                    {
                        SwapUpDown(P.row, P.column-1);

                        if ((!g.CheckInside(P.row, P.column) && !g.CheckInside(P.row, P.column - 1)) ||
                            g.IsFallingState(P.row, P.column))
                        {
                            SwapUpDown(P.row, P.column - 1);
                        }

                        if (g.CheckInside(P.row, P.column))
                        {
                            Score += g.Clear(P.row, P.column);
                        }

                        if (g.CheckInside(P.row, P.column - 1)) 
                        {
                            Score += g.Clear(P.row, P.column-1);
                        }

                        Hold = false;
                    }
                    else if (Control.Key is ConsoleKey.DownArrow && g.InsideGrid(P.row, P.column + 1))
                    {
                        SwapUpDown(P.row, P.column);

                        if ((!g.CheckInside(P.row, P.column) && !g.CheckInside(P.row, P.column + 1)) ||
                            g.IsFallingState(P.row, P.column)) 
                        {
                            SwapUpDown(P.row, P.column);
                        }

                        if (g.CheckInside(P.row, P.column))
                        {
                            Score += g.Clear(P.row, P.column);
                        }

                        if (g.CheckInside(P.row, P.column + 1))
                        {
                            Score += g.Clear(P.row, P.column+1);
                        }

                        Hold = false;
                    }

                    Falling = Fall_Speed-1;
                }
            }
        }
        void SHowGridBlocks() 
        {
            int r = 0, c = 0;

            foreach (int a in g.GridBlock())
            {
                Console.ForegroundColor = Color[a];
                Console.SetCursorPosition(r, c);
                Console.Write(a);

                r = (r + 1) % g.row;

                if (r is 0)
                {
                    c++;
                }
            }
        }
        void Move(int row, int column) 
        {
            P.row += row;
            P.column += column;
        }
        public void SwapLeftRight(int r, int c)
        {
            (g[c, r + 1], g[c, r]) = (g[c, r], g[c, r + 1]);
        }
        public void SwapUpDown(int r, int c)
        {
            (g[c + 1, r], g[c, r]) = (g[c, r], g[c + 1, r]);
        }
    }
    public class SGrid 
    {
        private readonly int[,] grid;
        public int this[int column, int row] 
        {
            get => grid[column, row];
            set => grid[column, row] = value;
        }
        public int row { get; set; }
        public int column { get; set; }
        private readonly Random random = new Random();
        public SGrid(int row, int column) 
        {
            this.row = row;
            this.column = column;
            grid = new int[column, row];
            ResetFill();
        }
        public IEnumerable<int> GridBlock() 
        {
            foreach (int n in grid) 
            {
                yield return n;
            }
        }
        void ResetFill()
        {
            for (int c = 0; c < column; c++)
            {
                for (int r = 0; r < row; r++)
                {
                    Scan(r,c);
                }
            }
        }
        void Scan(int r, int c) 
        {
            do
            {
                grid[c, r] = RandomNum(grid[c, r]);
            }
            while (CheckInside(r, c));
        }
        public bool CheckInside(int r, int c)
        {
            return CheckRow(r,c) || CheckColumn(r,c);
        }
        bool CheckRow(int r, int c)
        {
            if (InsideGrid(r + 2, c) && grid[c, r] == grid[c, r + 1] && grid[c, r + 1] == grid[c, r + 2])
            {
                return true;
            }
            else if (InsideGrid(r - 2, c) && grid[c, r] == grid[c, r - 1] && grid[c, r - 1] == grid[c, r - 2])
            {
                return true;
            }
            else if (InsideGrid(r-1,c) && InsideGrid(r+1,c) && grid[c,r] == grid[c,r-1] && grid[c,r] == grid[c,r+1]) 
            {
                return true;
            }
            return false;
        }
        bool CheckColumn(int r, int c) 
        {
            if (InsideGrid(r, c + 2) && grid[c, r] == grid[c + 1, r] && grid[c + 1, r] == grid[c + 2, r])
            {
                return true;
            }
            else if (InsideGrid(r, c - 2) && grid[c, r] == grid[c - 1, r] && grid[c - 1, r] == grid[c - 2, r])
            {
                return true;
            }
            else if (InsideGrid(r,c+1) && InsideGrid(r,c-1) && grid[c,r] == grid[c+1,r] && grid[c,r] == grid[c-1,r]) 
            {
                return true;
            }
            return false;
        }
        public bool InsideGrid(int r, int c) 
        {
            return r >= 0 && c >= 0 && r < row && c < column;
        }
        public bool Drop() //might need a specifc system for SR
        {
            bool drops = false;

            for (int r = 0; r < row; r++)
            {
                for (int c = column - 2; c >= 0; c--)
                {
                    if (grid[c + 1, r] is 0)
                    {
                        grid[c + 1, r] = grid[c, r];
                        grid[c, r] = 0;
                        drops = true;
                    }
                }

                FillGap(r);
            }

            return drops;
        }
        public int CheckEntireGrid()
        {
            int score = 0;

            for (int r = 0; r < row; r++) 
            {
                for (int c = 0; c < column; c++) 
                {
                    if ( CheckInside(r,c)) 
                    {
                        score += Clear(r,c);
                    }
                }
            }

            return score;
        }
        public bool IsFallingState(int r, int c) 
        {
            do
            {
                if (grid[c, r] is 0)
                {
                    return true;
                }

                c++;
            }
            while (c < column);
            
            return false;
        }
        public int Clear(int SR, int SC) 
        {
            int score = 0;

            List<int> list_r = new List<int>()
            {
                SR
            };
            List<int> list_c = new List<int>()
            {
                SC
            };

            for (int t = 0; t < (list_r.Count+list_c.Count)/2; t++)
            {
                if (InsideGrid(list_r[t]+1, list_c[t]) && grid[list_c[t], list_r[t]] == grid[list_c[t], list_r[t] + 1])
                {
                    if (!Duplicate(list_r[t]+1, list_c[t], list_r, list_c))
                    {
                        list_r.Add(list_r[t] + 1);
                        list_c.Add(list_c[t]);
                    }
                }
                if (InsideGrid(list_r[t]-1, list_c[t]) && grid[list_c[t], list_r[t]] == grid[list_c[t], list_r[t] - 1])
                {
                    if (!Duplicate(list_r[t]-1, list_c[t], list_r, list_c))
                    {
                        list_r.Add(list_r[t] - 1);
                        list_c.Add(list_c[t]);
                    }
                }
                if (InsideGrid(list_r[t], list_c[t]+1) && grid[list_c[t], list_r[t]] == grid[list_c[t] + 1, list_r[t]])
                {
                    if (!Duplicate(list_r[t], list_c[t]+1,list_r, list_c))
                    {
                        list_r.Add(list_r[t]);
                        list_c.Add(list_c[t] + 1);
                    }
                }
                if (InsideGrid(list_r[t], list_c[t]-1) && grid[list_c[t], list_r[t]] == grid[list_c[t] - 1, list_r[t]])
                {
                    if (!Duplicate(list_r[t], list_c[t]-1, list_r, list_c))
                    {
                        list_r.Add(list_r[t]);
                        list_c.Add(list_c[t] - 1);
                    }
                }
            }

            for (int a = ((list_r.Count+list_c.Count)/2)-1; a >= 0; a--) 
            {
                grid[list_c[a], list_r[a]] = 0;
                score += 100;
            }

            return score;
        }
        bool Duplicate(int x, int y, List<int> lr, List<int>lc) 
        {
            for (int a = (lr.Concat(lc).Count()/2)-1; a >= 0; a--) 
            {
                if (x == lr[a] && y == lc[a]) 
                {
                    return true;
                }
            }
            return false;
        }
        public void FillGap(int r) 
        {
            if (grid[0, r] is 0)
            {
                grid[0, r] = RandomNum(grid[0,r]);
            }
        }
        int RandomNum(int pre) 
        {
            int num;

            do
            {
                num = random.Next(1,6);
            }
            while (pre == num);

            return num;
        }
    }
    public class Pos 
    {
        public int row { get; set; }
        public int column { get; set; }
        public Pos(int row, int column) 
        {
            this.row = row;
            this.column = column;
        }
    }
}
