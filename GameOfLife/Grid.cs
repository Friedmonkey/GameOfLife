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

        public Grid(int size = 32) 
        {
            gridSize = size;
            data = new byte[size * size];
        }

        public void LoadData(byte[] data)
        { 
            this.data = data;
        }


        
        public void Step()
        {
            List<int> Flipped = new List<int>();

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int neighbors = Calculate(x, y, out int index);
                    bool alive = data[index] == 1;
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
                bool alive = data[Flipped[i]] == 1;
                byte status = (byte)(alive ? 0 : 1);

                data[Flipped[i]] = status;
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
                        if (data[newIndex] != 0)
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
                        writer.Write(GetValue(i, j) == 1 ? "1" : "0");
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
        }

        public byte GetValue(int x, int y)
        {
            // Calculate the index in the 1D array
            int index = x * gridSize * gridSize + y;

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
