using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

namespace cli_life
{
    public class GameSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    public class Cell
    {
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public int count1 = 0;
        public int count2 = 0;
        public readonly Cell[,] Cells;
        public readonly int CellSize;
        public void GetAverageStabilizationGeneration(int numGenerations)
        {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            if (count == count1)
                count2++;
            if (count2 >= 10)
                Console.WriteLine($"Stable phase on {numGenerations} generations");
            count1 = count;
        }
        public void ClassifyElements()
        {
            int blockCount = 0;
            int rows = Rows; // 20
            int cols = Columns; // 50
            bool[,] block = new bool[2, 2] { { true, true }, { true, true } };

            for (int r = 0; r < rows - block.GetLength(0); r++)
            {
                for (int c = 0; c < cols - block.GetLength(1); c++)
                {
                    bool match = true;
                    for (int br = 0; br < block.GetLength(0); br++)
                    {
                        for (int bc = 0; bc < block.GetLength(1); bc++)
                        {
                            if (Cells[c + bc, r + br].IsAlive != block[br, bc])
                            {
                                match = false;
                                break;
                            }
                        }
                        if (!match)
                            break;
                    }
                    if (match)
                    {
                        blockCount++;
                    }
                }
            }
            Console.WriteLine($"Number of elements matching the \"cube\" - {blockCount}");
        }
        public void GetTotalElementCount()
        {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            Console.WriteLine($"Number of combinations - {count}");
        }
       
        public void SaveToFile(string fileName)
        {
            var lines = new List<string>();
            for (int row = 0; row < Rows; row++)
            {
                var line = new StringBuilder();
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
                    line.Append(cell.IsAlive ? '*' : ' ');
                }
                lines.Add(line.ToString());
            }
            File.WriteAllLines(fileName, lines);
        }

        public void LoadFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            for (int row = 0; row < Rows; row++)
            {
                var line = lines[row];
                for (int col = 0; col < Columns; col++)
                {
                    var cell = Cells[col, row];
                    cell.IsAlive = line[col] == '*';
                }
            }
        }
        public void LoadColonyFromFile(string fileName, int x, int y)
        {
            var lines = File.ReadAllLines(fileName);
            for (int row = 0; row < lines.Length; row++)
            {
                var line = lines[row];
                for (int col = 0; col < line.Length; col++)
                {
                    var cell = Cells[x + col, y + row];
                    cell.IsAlive = line[col] == '*';
                }
            }
        }

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public void GetSymmetricElementCount()
        {
            int count = 0;
            for (int r = 0; r < Rows/2; r++)
            {
                for (int c = 0; c < Columns/2; c++)
                {
                    if (Cells[c,r].IsAlive && Cells[Columns - 1 - c, Rows - 1 - r].IsAlive)
                    {
                        count++;
                    }
                }
            }
            Console.WriteLine($"Number of symmetrical cells - {count}");
        }

        public void ExploreSymmetry(int numGenerations)
        {
            var board1 = this.Clone();
            var board2 = this.Clone();
            
            board1.Advance();
            board2.Advance();
            board2.Mirror();
            if (board1.Equals(board2))
                Console.WriteLine($"System is symmetrical on {numGenerations} generations.");
            else
            Console.WriteLine($"System is asymmetric on {numGenerations} generations.");
        }

        private void Mirror()
        {
            int rows = Rows;
            int cols = Columns;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols / 2; c++)
                {
                    bool temp = Cells[c, r].IsAlive;
                    Cells[c, r].IsAlive = Cells[cols - 1 - c, r].IsAlive;
                    Cells[cols - 1 - c, r].IsAlive = temp;
                }
            }
        }

        public Board Clone()
        {
            var clone = new Board(Width, Height, CellSize);
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    clone.Cells[x, y].IsAlive = Cells[x, y].IsAlive;
            return clone;
        }
    }
    class Program
    { 
        static Board board;
        static private void Reset()
        {
            string settingsJson = File.ReadAllText("settings.json");
            GameSettings settings = JsonConvert.DeserializeObject<GameSettings>(settingsJson);

            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);

            board.LoadColonyFromFile("colony1.txt", 25, 10);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
            int generation = 0;
            Reset();

            bool shouldContinue = true;
            while (shouldContinue)
            {
                Console.Clear();
                Render();
                generation++;
                board.Advance();
                board.GetTotalElementCount();
                board.ClassifyElements();
                board.GetSymmetricElementCount();
                board.ExploreSymmetry(generation);
                board.GetAverageStabilizationGeneration(generation);
                Console.WriteLine("S - save, L - load, or any other key to continue.");
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.S)
                {
                    Console.WriteLine("\nSaving to file...");
                    board.SaveToFile("game_state.txt");
                    Thread.Sleep(500);
                }
                else if (key == ConsoleKey.L)
                {
                    Console.WriteLine("\nLoading from file...");
                    board.LoadFromFile("game_state.txt");
                    Thread.Sleep(500);
                }
                else
                {
                    shouldContinue = true;
                }
            }
        }
    }
}
