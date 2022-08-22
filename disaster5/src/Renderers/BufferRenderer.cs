using System;
using System.Collections.Generic;
using Raylib_cs;

namespace Disaster
{
    public static class BufferRenderer
    {
        public static BlendMode blendMode { get; set; }
        public static bool inBuffer { get; private set; }
        private static string assetId;
        private static RenderTexture2D renderTexture;
        private static List<Action> drawQueue;

        public static bool StartBuffer(int width, int height)
        {
            if (inBuffer) return false;

            inBuffer = true;
            assetId = "";
            Raylib.UnloadRenderTexture(renderTexture);
            renderTexture = Raylib.LoadRenderTexture(width, height);
            Enqueue(() =>
            {
                Raylib.ClearBackground(Color.BLANK);
                Raylib.BeginBlendMode(blendMode);
            });
            return true;
        }

        public static bool StartBuffer(string assetID)
        {
            if (inBuffer) return false;

            var (succeeded, pixelBuffer) = Assets.PixelBuffer(assetID);
            if (!succeeded) return false;
            
            inBuffer = true;
            assetId = assetID;
            Raylib.UnloadRenderTexture(renderTexture);
            renderTexture = Raylib.LoadRenderTexture(pixelBuffer.width, pixelBuffer.height);
            Enqueue(() =>
            {
                Raylib.ClearBackground(Color.BLANK);
                Raylib.DrawTexture(pixelBuffer.texture, 0, 0, Color.WHITE);
                Raylib.BeginBlendMode(blendMode);
            });
            return true;
        }

        public static void Enqueue(Action renderAction)
        {
            if (!inBuffer) return;
            drawQueue ??= new List<Action>();
            drawQueue.Add(renderAction);
        }

        public static string EndBuffer()
        {
            if (!inBuffer) return "";
            inBuffer = false;

            // Draw everything!
            Raylib.BeginDrawing();
            Raylib.BeginTextureMode(renderTexture);
            drawQueue ??= new List<Action>();
            foreach (var action in drawQueue)
                action.Invoke();
            drawQueue.Clear();
            Raylib.EndBlendMode();
            Raylib.EndTextureMode();
            Raylib.EndDrawing();

            // Build the color array
            var image = Raylib.GetTextureData(renderTexture.texture);
            Raylib.ImageFlipVertical(ref image);
            var pixels = new Color32[image.width * image.height];
            unsafe
            {
                var colors = (Color32*) image.data;
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = colors[i];
            }

            if (assetId == "")
            {
                // Create pixelbuffer and it's id
                var output = new PixelBuffer(pixels, image.width);
                var hashnum = output.GetHashCode();
                while (Assets.pixelBuffers.ContainsKey(hashnum.ToString()))
                    hashnum += 1;
                assetId = hashnum.ToString();
                Assets.pixelBuffers.Add(assetId, output);
            }
            else
            {
                // We already have a pixel buffer, just update it's pixels
                var pixelBuffer = Assets.PixelBuffer(assetId).pixelBuffer;
                pixelBuffer.SetPixels(pixels);
                Assets.pixelBuffers[assetId] = pixelBuffer;
            }

            return assetId;
        }
    }
}