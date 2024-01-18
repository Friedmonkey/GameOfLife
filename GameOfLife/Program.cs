using FriedPixelWindow;
using SFML.Graphics;
using System.Collections;

internal class Program
{
    public const int waitTime = 250;
    public const int gridSize = 32;
    public const int size = gridSize * gridSize;
    public static bool[,] boolData = new bool[gridSize, gridSize];
    public static byte[] data = new byte[size];

    public static Dictionary<int, byte> binaryTranslation = new Dictionary<int, byte>()
    {
        { 0, 0b0000_0000 },
        { 1, 0b0000_0001 },
        { 2, 0b0000_0010 },
        { 3, 0b0000_0011 },
        { 4, 0b0000_0100 },
        { 5, 0b0000_0101 },
        { 6, 0b0000_0110 },
        { 7, 0b0000_0111 },
        { 8, 0b0000_1000 },
    };

    public static byte AliveRule = 0b000_1_0000;
    public static byte DeadRule = 0b000_0_0000;

    public static byte RuleMask = 0b000_1_0000;
    public static byte NeighborMask = 0b0000_1111;

    public static byte[] Rules = new byte[]
    {
        //First rule of Conways Game Of Life
        //any alive with 0 or 1 neighbors die
        0b000_1_0000,
        0b000_1_0001,


        //Second rule of Conways Game Of Life
        //any alive cell with exactly 2 neigbors stays alive
        //we dont need any because by rule of elimination well just ignore those cells

        //Third rule of Conways Game Of Life
        //any alive with 4,5,6,7 or 8 neighbors die
        0b000_1_0100,
        0b000_1_0101,
        0b000_1_0110,
        0b000_1_0111,
        0b000_1_1000,


        //Fourth rule of Conways Game Of Life
        //any dead cells with exacly 3 neigbor will be alive
        0b000_0_0011,
    };

    private static void Main(string[] args)
    {
        ReadData();
        ConvertBoolDataToByteArray();

        var appManager = new PixelManager();

        var window = new PixelWindow(gridSize, gridSize, 400/ gridSize, "Game of life", appManager,
            fixedTimestep: 20, framerateLimit: 300, debugInfo:false);

        Thread thread = new Thread(new ThreadStart(Stepper));
        thread.Start();

        window.Run();
    }
    public static void Stepper()
    {
        while (true)
        {
            Thread.Sleep(waitTime);
            List<(int, int)> Flipping = new List<(int, int)>();

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int neighbors = CalculateNeighbor(x, y);
                    foreach (var rule in Rules)
                    {
                        var Ncomp = (rule & NeighborMask);
                        var translation = binaryTranslation[neighbors];
                        if (Ncomp == translation)
                        {
                            var Rcomp = (rule & RuleMask);
                            var translation2 = (boolData[x, y] ? AliveRule : DeadRule);
                            if (Rcomp == translation2)
                            {
                                Flipping.Add((x, y));
                            }
                        }
                    }
                }
            }
            foreach (var (x, y) in Flipping)
            {
                boolData[x, y] = !boolData[x, y];
            }
            ConvertBoolDataToByteArray();
        }
    }
    public static int CalculateNeighbor(int x, int y)
    {
        int neighbors = 0;
        
        try { if (boolData[x-1, y-1]) neighbors++; } catch {}          
        try { if (boolData[x-1, y+0]) neighbors++; } catch {}
        try { if (boolData[x-1, y+1]) neighbors++; } catch {}

        try { if (boolData[x+0, y-1]) neighbors++; } catch {}
        //try { if (boolData[x-0, y+0]) neighbors++; } catch {}
        try { if (boolData[x+0, y+1]) neighbors++; } catch {}

        try { if (boolData[x+1, y-1]) neighbors++; } catch {}
        try { if (boolData[x+1, y+0]) neighbors++; } catch {}
        try { if (boolData[x+1, y+1]) neighbors++; } catch {}

        return neighbors;
    }
    public static void ConvertBoolDataToByteArray()
    {

        // Iterate through each row
        for (int x = 0; x < gridSize; x++)
        {
            // Iterate through each column
            for (int y = 0; y < gridSize; y++)
            {
                // Convert boolean to bytes
                byte value = boolData[x, y] ? (byte)1 : (byte)0;
                int dataIndex = (x * gridSize) + y  ;

                // Set the four consecutive bytes in the data array
                data[dataIndex] = value;
            }
        }
    }

    public static void ConvertByteArrayToBoolData()
    {
        int dataIndex = 0;

        // Iterate through each row
        for (int i = 0; i < gridSize; i++)
        {
            // Iterate through each column
            for (int j = 0; j < gridSize; j++)
            {
                // Use only the first byte to convert back to boolean
                boolData[i, j] = data[dataIndex] != 0;

                // Move to the next set of four bytes
                dataIndex += 1;
            }
        }
    }
    public static void ReadData()
    {
        if (!File.Exists("data.txt"))
        {
            File.Create("data.txt").Close();
            WriteData();
        }

        string content = File.ReadAllText("data.txt");

        int dataIndex = 0;

        // Iterate through each character
        for (int i = 0; i < content.Length && dataIndex < size; i++)
        {
            // Ignore white space
            if (!char.IsWhiteSpace(content[i]))
            {
                // Check the first bit
                boolData[dataIndex / gridSize, dataIndex % gridSize] = (content[i] & 1) == 1;
                dataIndex++;
            }
        }
    }

    public static void WriteData()
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
                    writer.Write(boolData[i, j] ? "1" : "0");
                }
                // Add a newline after each row
                writer.WriteLine();
            }
        }
    }
}
class PixelManager : IPixelWindowAppManager
{

    public void OnLoad(RenderWindow renderWindow)
    {
        // On load function - runs once at start.
        // The SFML render window provides ability to set up events and input (maybe store a reference to it for later use in your update functions)
    }

    public void Update(float frameTime)
    {
        // Update function - process update logic to run every frame here
    }

    public void FixedUpdate(float timeStep)
    {
        // Fixed update function - process logic to run every fixed timestep here
    }

    public void Render(PixelData pixelData, float frameTime)
    {
        // Render function - set pixel data for the current frame here
        // Randomised pixels shown as example.

        pixelData.Clear();
        pixelData.SetRawDataSpan(Program.data.AsSpan()); //using a span which i think is better
        //pixelData.SetRawData(Program.emu.data.Skip(0x0200).Take(1024).ToArray());
    }
}