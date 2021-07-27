using System.Numerics;
using System;
using Raylib_cs;
using System.Runtime.InteropServices;
namespace Disaster
{
    public class SoftwareCanvasRenderer : Renderer
    {
        Texture2D texture;
        public Shader shader;
        IntPtr pixels;

        public SoftwareCanvasRenderer(Shader shader)
        {
            texture = Raylib.LoadTextureFromImage(Raylib.GenImageChecked(ScreenController.screenWidth, ScreenController.screenHeight, 16, 16, Color.BLUE, Color.BROWN));
            Raylib.SetTextureFilter(texture, TextureFilter.TEXTURE_FILTER_POINT);

            this.shader = shader;
            pixels = Marshal.AllocHGlobal(SoftwareCanvas.textureWidth * SoftwareCanvas.textureHeight * 4);
        }

        public void Update()
        {
            unsafe
            {
                if (!SoftwareCanvas.overdraw)
                {
                    SoftwareCanvas.colorBuffer.AsSpan().CopyTo(new Span<Color32>((void*)pixels, SoftwareCanvas.textureWidth * SoftwareCanvas.textureHeight * 4));
                } else
                {
                    var buff = SoftwareCanvas.GetOverdrawColorBuffer();
                    buff.AsSpan().CopyTo(new Span<Color32>((void*)pixels, SoftwareCanvas.textureWidth * SoftwareCanvas.textureHeight * 4));
                }
            }
            Raylib.UpdateTexture(texture, pixels);
        }

        public void Render()
        {
            Raylib.BeginShaderMode(shader);
            Raylib.DrawTexturePro(
                texture,
                new Rectangle(0, 0, texture.width, texture.height),
                new Rectangle(0, 0, ScreenController.windowWidth, ScreenController.windowHeight),
                Vector2.Zero,
                0,
                Color.RAYWHITE
            );
            Raylib.EndShaderMode();
        }

        public void Dispose()
        {
            Raylib.UnloadTexture(texture);
            Raylib.UnloadShader(shader);
        }
    }
}