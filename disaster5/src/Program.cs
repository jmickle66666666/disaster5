// entry point

using System;
using System.IO;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

namespace Disaster
{
    
    class Program
    {

        static void LoadConfig()
        {
            string basedir = "";
            string[] lines = new string[] { };
            if (File.Exists("disaster.cfg"))
            {
                lines = File.ReadAllLines("disaster.cfg");
            } else
            {
                Console.WriteLine("Can't find disaster.cfg");
                Console.WriteLine("working path: " + Directory.GetCurrentDirectory());
            }

            foreach (var line in lines)
            {
                string[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "basedir":
                        if (tokens.Length != 2)
                        {
                            Console.WriteLine($"Unexpected number of tokens: {line}");
                            break;
                        }
                        basedir = tokens[1];
                        break;
                }
            }

            Assets.basePath = basedir;
        }

        public static ScreenController screen;
        static int loadingScreenPosition = 0;

        public static bool running = true;
        public static double timescale = 1;

        public static void LoadingMessage(string message)
        {
            LoadingMessage(message, new Color32(255, 140, 0));
        }

        public static void LoadingMessage(string message, Color32 color)
        {
            Console.WriteLine(message);
            string[] lines = SoftwareCanvas.SplitLineToFitScreen(message);
            foreach (var l in lines)
            {
                if (l == "") continue;
                SoftwareCanvas.Text(0, loadingScreenPosition, color, l);
                loadingScreenPosition += SoftwareCanvas.fontHeight;
            }
            screen.Update();
        }

        static void Main(string[] args)
        {
            if (Array.IndexOf(args, "-h") != -1 || Array.IndexOf(args, "--help") != -1)
            {
                Console.WriteLine("usage: disaster.exe [basepath]");
                return;
            }

            //Raylib.SetTraceLogLevel(TraceLogLevel.LOG_ALL);
            Raylib.SetTraceLogLevel(TraceLogLevel.LOG_ERROR);
            
            Raylib.InitAudioDevice();

            Console.WriteLine($"Welcome to disaster engine");
            LoadConfig();

            // handle arguments
            // arg0 = basepath override
            if (args.Length > 0)
            {
                Assets.basePath = args[0];
            }

            // software renderer initialisation
            SoftwareCanvas.InitTexture(320, 240);
            // TODO: bake in a default font! so you can't end up with no font at all
            if (Assets.LoadPath("lib/fontsmall.png", out string fontPath))
            {
                SoftwareCanvas.LoadFont(fontPath);
            } else
            {
                Console.WriteLine("load default font here");
                SoftwareCanvas.LoadDefaultFont();
            }
            screen = new ScreenController(320, 240, 2);

            LoadingMessage("disaster engine 5.0");
            LoadingMessage("(c) jazz mickle ultramegacorp 2021");
            LoadingMessage("initialised screen");
            var js = new JS();

            Debug.enabled = false;

            Raylib.SetExitKey(KeyboardKey.KEY_HOME);
            while (!Raylib.WindowShouldClose())
            {
                MusicController.Update();
                Debug.FrameStart();
                js.Update(Raylib.GetFrameTime());
                Debug.Label("js update");
                Debug.DrawGraph();
                screen.Update();
                SoftwareCanvas.slowDraw = false;
                Debug.FrameEnd();
                Debug.GetFrameMSData();
            }

            screen.Done();
            Raylib.CloseAudioDevice();
        }
    }
}
