using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;
using System.IO;
using System;
using Raylib_cs;
using System.Runtime.InteropServices;

namespace Disaster
{
    public struct PixelBuffer
    {
        public int width;
        public int height;
        public Color32[] pixels;
        public Texture2D texture;

        public PixelBuffer(Color32[] pixels, int width)
        {
            this.height = pixels.Length / width;
            var image = Raylib.GenImageColor(width, height, Color.MAGENTA);
            unsafe
            {
                pixels.AsSpan().CopyTo(new Span<Color32>((void*)image.data, width * height * 4));
            }
            this.width = width;
            this.pixels = pixels;
            this.texture = Raylib.LoadTextureFromImage(image);
        }

        public PixelBuffer(Color32[] pixels, int width, Texture2D texture)
        {
            this.width = width;
            this.height = pixels.Length / width;
            this.pixels = pixels;
            this.texture = texture;
        }

        public void SetPixels(Color32[] pixels)
        {
            if (pixels.Length != width * height)
            {
                Console.WriteLine($"Incorrect amount of pixels set on pixelbuffer: Expected {width * height} Got {pixels.Length}");
                return;
            }
            var image = Raylib.GenImageColor(width, height, Color.MAGENTA);
            unsafe
            {
                pixels.AsSpan().CopyTo(new Span<Color32>((void*)image.data, width * height * 4));
            }
            this.pixels = pixels;
            this.texture = Raylib.LoadTextureFromImage(image);
        }

        private static PixelBuffer _missing;
        private static bool _missingDefined = false;
        public static PixelBuffer missing
        {
            get
            {
                if (!_missingDefined)
                {
                    int mw = 16;
                    int mh = 16;
                    Color32[] pixels = new Color32[mw * mh];
                    for (int j = 0; j < mh; j++)
                    { 
                        for (int i = 0; i < mw; i++)
                        {
                            pixels[i + j * mw] = (i + j) % 8 < 4 ? new Color32(255, 160, 0) : new Color32(0, 0, 0);
                        }
                    }
                    var image = Raylib.GenImageColor(mw, mh, Color.MAGENTA);
                    unsafe
                    {
                        pixels.AsSpan().CopyTo(new Span<Color32>((void*)image.data, mw * mh * 4));
                    }
                    _missing = new PixelBuffer(pixels, mw);
                    _missingDefined = true;
                }
                return _missing;
            }
        }

        public string Serialise()
        {
            return pixels.GetHashCode().ToString();
        }
    }

    public class Assets
    {
        public static string basePath = "";

        static List<string> missingAssetPaths;

        public static Dictionary<string, ObjectInstance> scripts;
        public static List<string> currentlyLoadingScripts;
        public static Dictionary<string, Texture2D> textures;
        public static Dictionary<string, PixelBuffer> pixelBuffers;
        public static Dictionary<string, Model> models;
        public static Dictionary<string, Sound> audio;
        public static Dictionary<string, Music> music;
        public static Dictionary<string, Shader> shaders;
        public static Dictionary<string, string> texts;

        public static bool assignedDefaultShader = false;
        static Shader _defaultShader;
        public static Shader defaultShader
        {
            get
            {
                if (!assignedDefaultShader)
                {
                    _defaultShader = Shader("shaders/model").shader;
                    assignedDefaultShader = true;
                }
                return _defaultShader;
            }
        }

        public static void InitDictionaries()
        {
            scripts = new Dictionary<string, ObjectInstance>();
            textures = new Dictionary<string, Texture2D>();
            pixelBuffers = new Dictionary<string, PixelBuffer>();
            models = new Dictionary<string, Model>();
            audio = new Dictionary<string, Sound>();
            music = new Dictionary<string, Music>();
            shaders = new Dictionary<string, Shader>();
            texts = new Dictionary<string, string>();
            currentlyLoadingScripts = new List<string>();
        }

        public static int TotalLoaded()
        {
            int count = 0;
            count += scripts.Count;
            count += pixelBuffers.Count;
            count += models.Count;
            count += audio.Count;
            count += music.Count;
            count += shaders.Count;
            count += texts.Count;
            return count;
        }

        public static bool PathExists(string path)
        {
            path = Path.Combine(basePath, Reslash(path));
            return File.Exists(path);
        }

        public static bool LoadPath(string path, out string assetPath)
        {
            path = Reslash(path);
            var output = Path.Combine(basePath, path);
            assetPath = output;
            if (!File.Exists(output))
            {
                if (missingAssetPaths == null) missingAssetPaths = new List<string>();
                if (!missingAssetPaths.Contains(output))
                {
                    missingAssetPaths.Add(output);
                    Console.WriteLine($"Can't find asset: {output}");
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string Reslash(string path)
        {
            path = path.Replace('/', Path.AltDirectorySeparatorChar);
            path = path.Replace('\\', Path.AltDirectorySeparatorChar);
            return path;
        }

        public static bool Loaded(string path)
        {
            if (scripts.ContainsKey(path)) return true;
            if (pixelBuffers.ContainsKey(path)) return true;
            if (audio.ContainsKey(path)) return true;
            if (music.ContainsKey(path)) return true;
            if (texts.ContainsKey(path)) return true;
            return false;
        }

        public static void UnloadAll()
        {
            Dispose();
            scripts.Clear();
            pixelBuffers.Clear();
            
            AudioController.StopAllSound();
            foreach (var a in audio)
            {
                Raylib.UnloadSound(a.Value);
            }

            audio.Clear();
            music.Clear();
            texts.Clear();
            models.Clear();

            //foreach (var s in shaders)
            //{
            //    Raylib.UnloadShader(s.Value);
            //}
            shaders.Clear();
            //assignedDefaultShader = false;

            GC.Collect();
        }

        public static void Unload(string path)
        {
            path = Reslash(path);

            if (pixelBuffers.ContainsKey(path)) { pixelBuffers.Remove(path); }
            if (scripts.ContainsKey(path)) { scripts.Remove(path); }
            if (audio.ContainsKey(path)) { audio.Remove(path); }
            if (music.ContainsKey(path)) { music.Remove(path); }
            if (texts.ContainsKey(path)) { texts.Remove(path); }
            if (shaders.ContainsKey(path)) { Raylib.UnloadShader(shaders[path]); shaders.Remove(path); }
            if (models.ContainsKey(path)) { Raylib.UnloadModel(models[path]); models.Remove(path); }
        }

        public static void Preload(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".txt":
                    Text(path);
                    break;
                case ".png":
                    PixelBuffer(path);
                    break;
                case ".wav":
                    Audio(path);
                    break;
                case ".frag":
                case ".vert":
                    Shader(path);
                    break;
                case ".ogg":
                case ".mp3":
                    Music(path);
                    break;
            }
        }

        public static string[] GetAllPaths()
        {
            string[] output = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = Reslash(Path.GetRelativePath(basePath, output[i]));
            }
            return output;
        }

        public static string[] ListDir(string path, bool subdirectories)
        {
            string[] output = Directory.GetFiles(basePath + path, "*.*", subdirectories?SearchOption.AllDirectories:SearchOption.TopDirectoryOnly);
            int len = basePath.Length + 1;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = "\\" + output[i].Substring(len);
            }
            return output;
        }

        public static (bool succeeded, Shader shader) Shader(string path)
        {
            path = Reslash(path);
            if (!shaders.ContainsKey(path))
            {
                var vertFound = LoadPath(path + ".vert", out string vertPath);
                var fragFound = LoadPath(path + ".frag", out string fragPath);
                if (!vertFound || !fragFound)
                {
                    if (!vertFound) { Console.WriteLine($"No shader: {path}.vert"); }
                    if (!fragFound) { Console.WriteLine($"No shader: {path}.vert"); }
                    return (false, _defaultShader);
                }

                var output = Raylib.LoadShader(vertPath, fragPath);
                shaders.Add(path, output);
            }
            return (true, shaders[path]);
        }

        public static (bool succeeded, string text) Text(string path)
        {
            path = Reslash(path);
            if (!texts.ContainsKey(path))
            {
                if (!LoadPath(path, out string textPath))
                {
                    Console.WriteLine($"No file at: {path}");
                    return (false, "");
                }

                string output = File.ReadAllText(textPath);
                texts.Add(path, output);
            }
            return (true, texts[path]);
        }

        public static (bool succeeded, PixelBuffer pixelBuffer) PixelBuffer(string path)
        {
            path = Reslash(path);
            if (!pixelBuffers.ContainsKey(path))
            {
                if (!LoadPath(path, out string pixelBufferPath))
                {
                    return (false, Disaster.PixelBuffer.missing);
                }

                var image = Raylib.LoadImage(pixelBufferPath);
                Raylib.ImageFormat(ref image, PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8);

                Color32[] pixels = new Color32[image.width * image.height];
                unsafe
                {
                    var colors = ((Color32*)image.data);
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i] = colors[i];
                    }
                }

                var texture = Raylib.LoadTextureFromImage(image);

                var pixelBuffer = new PixelBuffer(
                    pixels,
                    image.width,
                    texture
                );
                pixelBuffers.Add(path, pixelBuffer);
            }
            return (true, pixelBuffers[path]);
        }

        public static (bool succeeded, Model model) Model(string path)
        {
            path = Reslash(path);
            if (!models.ContainsKey(path))
            {
                if (!LoadPath(path, out string modelPath))
                {
                    Program.LoadingMessage($"No model, bud. {modelPath}");
                    return (false, new Model());
                }
                Console.WriteLine($"loading: {modelPath}");
                var model = Raylib.LoadModel(modelPath);
                models.Add(path, model);
            }
            return (true, models[path]);
        }

        public static (bool succeeded, ObjectInstance script) Script(string path)
        {
            path = Reslash(path);
            if (!scripts.ContainsKey(path))
            {
                var newScript = LoadScript(path);
                if (newScript.succeeded)
                {
                    newScript.script.SetPropertyValue("path", path, false);
                    scripts.Add(path, newScript.script);
                } else
                {
                    return (false, null);
                }
            }

            return (true, scripts[path]);
        }

        public static (bool succeeded, ObjectInstance script) LoadScript(string path)
        {
            path = Reslash(path);
            if (!LoadPath(path, out string scriptPath))
            {
                return (false, null);
            }

            if (currentlyLoadingScripts.Contains(scriptPath))
            {
                Console.WriteLine($"Circular dependency: {scriptPath}");
                return (false, null);
            }

            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Cannot find script: {scriptPath}");
                return (false, null);
            }

            currentlyLoadingScripts.Add(scriptPath);

            var newEngine = new ScriptEngine();
            JS.LoadStandardFunctions(newEngine);
            newEngine.Execute(File.ReadAllText(scriptPath));
            
            currentlyLoadingScripts.Remove(scriptPath);

            return (true, newEngine.Global);
        }

        public static (bool succeeded, Music music) Music(string path)
        {
            path = Reslash(path);
            if (!music.ContainsKey(path))
            {
                if (!LoadPath(path, out string audioPath))
                {
                    Program.LoadingMessage($"No music, bud. {audioPath}");
                    return (false, new Music());
                }
                var newAudio = Raylib.LoadMusicStream(audioPath);
                music.Add(path, newAudio);
            }
            return (true, music[path]);
        }

        public static (bool succeeded, Sound sound) Audio(string path)
        {
            path = Reslash(path);
            if (!audio.ContainsKey(path))
            {
                if (!LoadPath(path, out string audioPath))
                {
                    Program.LoadingMessage($"No sound, bud. {audioPath}");
                    return (false, new Sound());
                }
                var newAudio = Raylib.LoadSound(audioPath);
                audio.Add(path, newAudio);
            }
            return (true, audio[path]);
        }

        public static void Dispose()
        {
           
        }
    }
}