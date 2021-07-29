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
        public static string basePath;

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
                    _defaultShader = Shader("shaders/model");
                    assignedDefaultShader = true;
                }
                return _defaultShader;
            }
        }

        public static bool LoadPath(string path, out string assetPath)
        {
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
            if (scripts != null && scripts.ContainsKey(path)) return true;
            if (pixelBuffers != null && pixelBuffers.ContainsKey(path)) return true;
            if (audio != null && audio.ContainsKey(path)) return true;
            if (music != null && music.ContainsKey(path)) return true;
            if (texts != null && texts.ContainsKey(path)) return true;
            return false;
        }

        public static void UnloadAll()
        {
            Dispose();
            if (scripts != null) scripts.Clear();
            if (pixelBuffers != null) pixelBuffers.Clear();
            if (audio != null) audio.Clear();
            if (music != null) music.Clear();
            if (texts != null) texts.Clear();
            if (models != null) models.Clear();

            if (shaders != null)
            {
                foreach (var s in shaders)
                {
                    Raylib.UnloadShader(s.Value);
                }
                shaders.Clear();
            }
            GC.Collect();
        }

        public static void Unload(string path)
        {
            path = Reslash(path);
            if (pixelBuffers == null) pixelBuffers = new Dictionary<string, PixelBuffer>();
            if (scripts == null) scripts = new Dictionary<string, ObjectInstance>();
            if (audio == null) audio = new Dictionary<string, Sound>();
            if (music == null) music = new Dictionary<string, Music>();
            if (texts == null) texts = new Dictionary<string, string>();
            if (models == null) models = new Dictionary<string, Model>();
            if (pixelBuffers.ContainsKey(path)) { pixelBuffers.Remove(path); }
            if (scripts.ContainsKey(path)) { scripts.Remove(path); }
            if (audio.ContainsKey(path)) { audio.Remove(path); }
            if (music.ContainsKey(path)) { music.Remove(path); }
            if (texts.ContainsKey(path)) { texts.Remove(path); }
            if (shaders.ContainsKey(path)) { Raylib.UnloadShader(shaders[path]); shaders.Remove(path); }
            if (models.ContainsKey(path)) { models.Remove(path); }
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
            int len = basePath.Length + 1;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = output[i].Substring(len);
            }
            return output;
        }

        public static Shader Shader(string path)
        {
            path = Reslash(path);
            if (shaders == null) shaders = new Dictionary<string, Shader>();
            if (!shaders.ContainsKey(path))
            {
                var vertFound = LoadPath(path + ".vert", out string vertPath);
                var fragFound = LoadPath(path + ".frag", out string fragPath);
                if (!vertFound || !fragFound)
                {
                    if (!vertFound) { Console.WriteLine($"No shader: {path}.vert"); }
                    if (!fragFound) { Console.WriteLine($"No shader: {path}.vert"); }
                    return _defaultShader;
                }

                var output = Raylib.LoadShader(vertPath, fragPath);
                shaders.Add(path, output);
            }
            return shaders[path];
        }

        public static string Text(string path)
        {
            path = Reslash(path);
            if (texts == null) texts = new Dictionary<string, string>();
            if (!texts.ContainsKey(path))
            {
                if (!LoadPath(path, out string textPath))
                {
                    return null;
                }

                string output = File.ReadAllText(textPath);
                texts.Add(path, output);
            }
            return texts[path];
        }

        public static PixelBuffer PixelBuffer(string path)
        {
            path = Reslash(path);
            if (pixelBuffers == null) pixelBuffers = new Dictionary<string, PixelBuffer>();
            if (!pixelBuffers.ContainsKey(path))
            {
                if (!LoadPath(path, out string pixelBufferPath))
                {
                    return Disaster.PixelBuffer.missing;
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
            return pixelBuffers[path];
        }

        public static Model Model(string path)
        {
            path = Reslash(path);
            if (models == null) models = new Dictionary<string, Model>();
            if (!models.ContainsKey(path))
            {
                if (!LoadPath(path, out string modelPath))
                {
                    Program.LoadingMessage($"No model, bud. {modelPath}");
                }
                Console.WriteLine($"loading: {modelPath}");
                var model = Raylib.LoadModel(modelPath);
                models.Add(path, model);
            }
            return models[path];
        }

        public static ObjectInstance Script(string path)
        {
            path = Reslash(path);
            if (scripts == null) scripts = new Dictionary<string, ObjectInstance>();
            if (!scripts.ContainsKey(path))
            {
                var newScript = LoadScript(path);
                if (newScript != null) scripts.Add(path, newScript);
            }

            return scripts[path];
        }

        public static ObjectInstance LoadScript(string path)
        {
            path = Reslash(path);
            if (!LoadPath(path, out string scriptPath))
            {
                return null;
            }

            if (currentlyLoadingScripts == null) currentlyLoadingScripts = new List<string>();

            if (currentlyLoadingScripts.Contains(scriptPath))
            {
                Console.WriteLine($"Circular dependency: {scriptPath}");
                return null;
            }

            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Cannot find script: {scriptPath}");
                return null;
            }

            currentlyLoadingScripts.Add(scriptPath);

            var newEngine = new ScriptEngine();
            JS.LoadStandardFunctions(newEngine);
            newEngine.Execute(File.ReadAllText(scriptPath));
            
            currentlyLoadingScripts.Remove(scriptPath);

            return newEngine.Global;
        }

        public static Music Music(string path)
        {
            path = Reslash(path);
            if (music == null) music = new Dictionary<string, Music>();
            if (!music.ContainsKey(path))
            {
                if (!LoadPath(path, out string audioPath))
                {
                    Program.LoadingMessage($"No music, bud. {audioPath}");
                }
                var newAudio = Raylib.LoadMusicStream(audioPath);
                music.Add(path, newAudio);
            }
            return music[path];
        }

        public static Sound Audio(string path)
        {
            path = Reslash(path);
            if (audio == null) audio = new Dictionary<string, Sound>();
            if (!audio.ContainsKey(path))
            {
                if (!LoadPath(path, out string audioPath))
                {
                    Program.LoadingMessage($"No sound, bud. {audioPath}");
                }
                var newAudio = Raylib.LoadSound(audioPath);
                audio.Add(path, newAudio);
            }
            return audio[path];
        }

        public static void Dispose()
        {
           
        }
    }
}