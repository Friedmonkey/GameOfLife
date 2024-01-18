﻿using FriedPixelWindow;
using GameOfLife;
using SFML.Graphics;
using SFML.Window;
using System.Collections;

internal class Program
{
    public const int waitTime = 50;
    public const int gridSize = 100;
    public const int Scale = 800;
    public const int size = gridSize * gridSize;
    public static Grid Grid = new Grid(gridSize);
    public static bool running = true;
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
        }
    }

    private static void RW_MouseButtonPressed(object? sender, MouseButtonEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"MouseDown X:{e.X}, Y:{e.Y}");

        Console.ForegroundColor = ConsoleColor.Red;
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