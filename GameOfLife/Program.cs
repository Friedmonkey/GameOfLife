using FriedPixelWindow;
using GameOfLife;
using SFML.Graphics;
using SFML.Window;
using System.Collections;

internal class Program
{
    public const int waitTime = 50;
    public const int gridSize = 100;
    public const int Scale = 1000;
    public const int size = gridSize * gridSize;
    public static Grid Grid = new Grid(gridSize);
    public static bool running = false;
    public static RenderWindow RW;

    private static void Main(string[] args)
    {
        Grid.ReadData();

        var appManager = new PixelManager();

        var window = new PixelWindow(gridSize, gridSize, Scale / gridSize, "Game of life", appManager,
            fixedTimestep: 20, framerateLimit: 300, debugInfo:false);

        RW = window.GetRenderWindow();
        RW.MouseButtonPressed += RW_MouseButtonPressed;


        Thread thread = new Thread(new ThreadStart(Stepper));
        thread.Start();

        Thread input = new Thread(new ThreadStart(Input));
        input.Start();

        window.Run();
    }
    public static void Input() 
    {
        while (true)
        {
            Thread.Sleep(1);
            bool spacedown = Keyboard.IsKeyPressed(Keyboard.Key.Space);
            if (spacedown)
            {
                running = !running;
                Console.WriteLine($"Game {(running ? "Resumed" : "Paused")}!");
                Thread.Sleep(200);
            }
            bool Rdown = Keyboard.IsKeyPressed(Keyboard.Key.R);
            if (Rdown)
            {
                bool wasdown = running;
                running = false;
                Console.WriteLine($"Importing data..");
                Grid.ReadData();
                Thread.Sleep(10);
                Console.WriteLine($"Data has been imported");
                if (wasdown)
                    running = true;
                Thread.Sleep(400);
            }
            bool Edown = Keyboard.IsKeyPressed(Keyboard.Key.E);
            if (Edown)
            {
                bool wasdown = running;
                running = false;
                Console.WriteLine($"Exporting data..");
                Grid.WriteData();
                Thread.Sleep(10);
                Console.WriteLine($"Data has been exported");
                if (wasdown)
                    running = true;
                Thread.Sleep(400);
            }
            bool Cdown = Keyboard.IsKeyPressed(Keyboard.Key.C);
            if (Cdown)
            {
                running = false;
                Grid.Fill(0);
                Console.WriteLine($"Screen Cleared!");
                Thread.Sleep(400);
            }
            bool Fdown = Keyboard.IsKeyPressed(Keyboard.Key.F);
            if (Fdown)
            {
                running = false;
                Grid.Fill(1);
                Console.WriteLine($"Screen Filled!");
                Thread.Sleep(400);
            }
            bool Sdown = Keyboard.IsKeyPressed(Keyboard.Key.S);
            if (Sdown)
            {
                running = false;
                Console.WriteLine($"Stepping");
                Thread.Sleep(10);
                Grid.Step();
                Thread.Sleep(200);
            }
            bool Ddown = Keyboard.IsKeyPressed(Keyboard.Key.D);
            if (Ddown)
            {
                Grid.Debug = !Grid.Debug;
                Console.WriteLine($"Debug {(running ? "Enabled" : "Disabled")}!");
                Thread.Sleep(200);
            }
            bool Vdown = Keyboard.IsKeyPressed(Keyboard.Key.V);
            if (Vdown)
            {
                Grid.PreCalc();
                Grid.Debug = true;
                Console.WriteLine($"Debug Enabled");
                Console.WriteLine("Calculated the cells");
                Thread.Sleep(200);
            }
        }
    }

    private static void RW_MouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        //Console.WriteLine($"MouseDown X:{e.X}, Y:{e.Y}");

        //Console.ForegroundColor = ConsoleColor.Red;
        var newX = e.Y / (Scale / gridSize);
        var newY = e.X / (Scale / gridSize);
        Console.WriteLine($"MouseDown X:{newX}, Y:{newY}");
        Console.ResetColor();

        byte value = (byte)(e.Button == Mouse.Button.Left ? 1 : 0);

        Grid.SetValue(newX,newY,value);
    }

    public static void Stepper()
    {
        while (true)
        {
            Thread.Sleep(waitTime);
            if (running)
            { 
                Grid.Step();
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
        pixelData.Clear();
        pixelData.SetRawDataSpan(Program.Grid.data);
    }
}