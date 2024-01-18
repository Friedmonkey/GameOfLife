using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Grid
    {
        private int gridSize = 32;
        public byte[] data = new byte[1024];

        public static bool Debug = false;

        public Grid(int size = 32) 
        {
            gridSize = size;
            data = new byte[size * size];
        }

        public void LoadData(byte[] data)
        { 
            this.data = data;
        }

        private static bool isAlive(byte value)
        {
            if (value == 0) return false;
            if (value == 1) return true;
            if (value == 2) return true;
            if (value == 5) return false;
            return false;
        }
        
        public void Step()
        {
            List<int> Flipped = new List<int>();

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int neighbors = Calculate(x, y, out int index);
                    bool alive = isAlive(data[index]);
                    if (alive && neighbors < 2)
                        Flipped.Add(index);
                    if (!alive && neighbors == 3)
                        Flipped.Add(index);
                    if (alive && neighbors > 3)
                        Flipped.Add(index);
                }
            }

            for (int i = 0; i < Flipped.Count; i++)
            {
                bool alive = isAlive(data[Flipped[i]]);
                byte status = (byte)(alive ? 0 : 1);

                data[Flipped[i]] = status;
            }
            if (Debug)
                PreCalc();
        }

        public void PreCalc()
        {
            List<int> AboutFlipped = new List<int>();

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int neighbors = Calculate(x, y, out int index);
                    bool alive = isAlive(data[index]);
                    if (alive)
                        data[index] = 1; //set it to one to make sure
                    if (alive && neighbors < 2)
                        AboutFlipped.Add(index);
                    if (!alive && neighbors == 3)
                        AboutFlipped.Add(index);
                    if (alive && neighbors > 3)
                        AboutFlipped.Add(index);
                }
            }

            for (int i = 0; i < AboutFlipped.Count; i++)
            {
                bool alive = isAlive(data[AboutFlipped[i]]);
                byte status = (byte)(alive ? 2 : 5);

                data[AboutFlipped[i]] = status;
            }

        }

        public int Calculate(int x, int y, out int index)
        {
            int neighbors = 0;
            index = -1;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    // Check if indices are within bounds
                    if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
                    {
                        // Calculate the index in the 1D array
                        int newIndex = newX * gridSize + newY;

                        // Skip the center cell
                        if (dx == 0 && dy == 0)
                        { 
                            index = newIndex;  
                            continue;
                        }

                        // Check the value in the 1D array
                        if (isAlive(data[newIndex]))
                        {
                            neighbors++;
                        }
                    }
                }
            }

            return neighbors;
        }

        //public int CalculateNeighbor(int x, int y)
        //{
        //    int neighbors = 0;

        //    for (int dx = -1; dx <= 1; dx++)
        //    {
        //        for (int dy = -1; dy <= 1; dy++)
        //        {
        //            if (dx == 0 && dy == 0) continue;  // Skip the center cell

        //            int newX = x + dx;
        //            int newY = y + dy;

        //            // Check if indices are within bounds
        //            if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        //            {
        //                // Calculate the index in the 1D array
        //                int newIndex = newX * gridSize + newY;

        //                // Check the value in the 1D array
        //                if (data[newIndex] != 0)
        //                {
        //                    neighbors++;
        //                }
        //            }
        //        }
        //    }

        //    return neighbors;
        //}

        public void ReadData()
        {
            if (!File.Exists("data.txt"))
            {
                File.Create("data.txt").Close();
                WriteData();
            }

            string content = File.ReadAllText("data.txt");

            int dataIndex = 0;

            // Iterate through each character
            for (int i = 0; i < content.Length && dataIndex < gridSize * gridSize; i++)
            {
                // Ignore white space
                if (!char.IsWhiteSpace(content[i]))
                {
                    // Check the first bit
                    SetValue(dataIndex / gridSize, dataIndex % gridSize, (byte)(content[i] & 1));
                    dataIndex++;
                }
            }
            if (Debug)
                PreCalc();
        }

        public void WriteData()
        {
            using (StreamWriter writer = new StreamWriter("data.txt"))
            {
                // Iterate through each row
                for (int i = 0; i < gridSize; i++)
                {
                    // Iterate through each column
                    for (int j = 0; j < gridSize; j++)
                    {
                        // Write 1 or 0 to the file with space
                        writer.Write(GetValue(i, j) == 0 ? "0" : "1");
                    }
                    // Add a newline after each row
                    writer.WriteLine();
                }
            }
        }

        public void Fill(byte Value)
        {
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                data[i] = Value;
            }
            if (Debug)
                PreCalc();
        }

        public byte GetValue(int x, int y)
        {
            // Calculate the index in the 1D array
            int index = x * gridSize + y;

            // Access the value in the 1D array
            return data[index];
        }

        public void SetValue(int x, int y, byte value)
        {
            // Calculate the index in the 1D array
            int index = x * gridSize + y;

            // Set the value in the 1D array
            data[index] = value;
        }
    }
}
