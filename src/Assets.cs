using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;
using System.IO;
using System;
using OpenGL;
using SDL2;

namespace Disaster 
{
    public struct PixelBuffer
    {
        public int width;
        public int height;
        public Color32[] pixels;
        public PixelBuffer(Color32[] pixels, int width)
        {
            this.width = width;
            this.height = pixels.Length / width;
            this.pixels = pixels;
        }
    }

    public class Assets {
        public static string basePath;

        public static Dictionary<string, ObjectInstance> scripts;
        public static List<string> currentlyLoadingScripts;
        public static Dictionary<string, Texture> textures;
        public static Dictionary<string, PixelBuffer> pixelBuffers;
        public static Dictionary<string, ObjModel> objModels;
        public static Dictionary<string, IntPtr> audio;
        public static Dictionary<string, IntPtr> music;

        static ShaderProgram _defaultShader;
        public static ShaderProgram defaultShader {
            get {
                if (_defaultShader == null) {
                    var vertShader = File.ReadAllText(LoadPath("vert.glsl"));
                    var fragShader = File.ReadAllText(LoadPath("frag.glsl"));
                    _defaultShader = new ShaderProgram(vertShader, fragShader);
                }
                return _defaultShader;
            }
        }

        public static string LoadPath(string path)
        {
            var output = Path.Combine(basePath, path);
            if (!File.Exists(output)) {
                Console.WriteLine($"No File: {output}");
            }
            return output;
        }

        public static Texture Texture(string path)
        {
            if (textures == null) textures = new Dictionary<string, Texture>();
            if (!textures.ContainsKey(path)) {
                var texturePath = LoadPath(path);
                
                SDL2.SDL.SDL_Surface surface = System.Runtime.InteropServices.Marshal.PtrToStructure<SDL2.SDL.SDL_Surface>(SDL2.SDL_image.IMG_Load(texturePath));
                var texture = new Texture(surface.pixels, surface.w, surface.h);
                textures.Add(path, texture);
            }
            return textures[path];
        }

        public static PixelBuffer PixelBuffer(string path)
        {
            if (pixelBuffers == null) pixelBuffers = new Dictionary<string, PixelBuffer>();
            if (!pixelBuffers.ContainsKey(path))
            {
                var pixelBufferPath = LoadPath(path);

                var surface = System.Runtime.InteropServices.Marshal.PtrToStructure<SDL.SDL_Surface>(
                    SDL_image.IMG_Load(pixelBufferPath)
                );

                Color32[] pixels = new Color32[surface.w * surface.h];
                unsafe
                {
                    var colors = ((Color32*)surface.pixels);
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i] = colors[i];
                    }
                }

                var pixelBuffer = new PixelBuffer(
                    pixels,
                    surface.w
                );
                pixelBuffers.Add(path, pixelBuffer);
            }
            return pixelBuffers[path];
        }

        public static ObjModel ObjModel(string path)
        {
            if (objModels == null) objModels = new Dictionary<string, ObjModel>();
            if (!objModels.ContainsKey(path))
            {
                var objModelPath = LoadPath(path);

                var objModel = Disaster.ObjModel.Parse(objModelPath);
                objModels.Add(path, objModel);
            }
            return objModels[path];
        }

        public static ObjectInstance Script(string path)
        {
            if (scripts == null) scripts = new Dictionary<string, ObjectInstance>();
            if (!scripts.ContainsKey(path)) {
                var scriptPath = LoadPath(path);

                if (currentlyLoadingScripts == null) currentlyLoadingScripts = new List<string>();

                if (currentlyLoadingScripts.Contains(scriptPath)) {
                    Console.WriteLine($"Circular dependency: {scriptPath}");
                    return null;
                }

                if (!File.Exists(scriptPath)) {
                    Console.WriteLine($"Cannot find script: {scriptPath}");
                    return null;
                }

                currentlyLoadingScripts.Add(scriptPath);

                var newEngine = new ScriptEngine();
                JS.LoadStandardFunctions(newEngine);
                newEngine.Execute(File.ReadAllText(scriptPath));
                scripts.Add(path, newEngine.Global);
                
                currentlyLoadingScripts.Remove(scriptPath);
            }

            return scripts[path];
        }

        public static IntPtr Music(string path)
        {
            if (music == null) music = new Dictionary<string, IntPtr>();
            if (!music.ContainsKey(path))
            {
                var audioPath = LoadPath(path);
                var newAudio = SDL_mixer.Mix_LoadMUS(Assets.LoadPath(audioPath));
                music.Add(path, newAudio);
            }
            return music[path];
        }

        public static IntPtr Audio(string path)
        {
            if (audio == null) audio = new Dictionary<string, IntPtr>();
            if (!audio.ContainsKey(path))
            {
                var audioPath = LoadPath(path);
                var newAudio = SDL_mixer.Mix_LoadWAV(Assets.LoadPath(audioPath));
                audio.Add(path, newAudio);
            }
            return audio[path];
        }

        public static void Dispose()
        {
            if (textures != null)
            {
                foreach (var t in textures.Values)
                {
                    t.Dispose();
                }
            }
        }
    }
}