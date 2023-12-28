using System;
using System.Collections;
using System.Collections.Generic;
using Raylib_cs;

namespace Disaster
{
    public static class TextController
    {
        public static int fontWidth;
        public static int fontHeight;

        private static bool[,] _fontBuffer;
        private static readonly Dictionary<string, (int width, int height, bool[,] data)> _fontCache = new Dictionary<string, (int width, int height, bool[,] data)>();
        private static readonly Color32[] RainbowColors = {Colors.red, Colors.meat, Colors.orange, Colors.yellow, Colors.slimegreen, Colors.skyblue};
        private const string DEFAULT_FONT_BASE64 = "AAAAAAAAwAEAAAAAAQYAAAAAAAIAAAAAhxMchkMoifMghAAASRIwQaJURiIQBAEASZIMQRJFRkIQAAEAh3E4RxJFSfIIAAIAAAAAAQAAAAAQBNEAAAAAAQAAAAAghGABAAAAAAAcAGAAAAAAAAAAABAgAJAAAAAAgGMYjhE4iYAUQpQYQJIESTAkiYAUQZQkQJIEyRMkiYAMQZUkgHEYjmE4BwAUwXIYCBAACAAAgYAEAQAABBAACAAAAQAEAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB8gZMYhEFEkfE4kAMAQZEkRKJsURIICAIAQZIgRBJVCiIIBAIAR3IYRBJFhEMIBAIASZIERBJFSoIIAqIAh3E4XxJFUfI4gUMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAATnIYxxMYyWEkT5QYQ5IkSRAkiZAkQZQk1ZMESRA0iYAUQZQkVXIEyXEEj4AMQdUkk5IkSRAkiYAUwbYkDnMYx/MYyfEkQZQYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAxvEYiGEEhgEMBCAYiSAkSJIESSIIhmMAiUAgD5IICQIAA8AYiYAQwnEQhiMIhmMgyZAkRBAgSQIABCAkhmAYyOM8hgEAAAAYAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAEoROYBiAAABEAEAAB8jrYABAEQAAAIAAEoHPEBBgM4gAMIAAEoB2EABqMQAAAQAKF8zvIQBEEAAAAQAKEoxGQQiKAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        public static int MaxTextLength()
        {
            return ScreenController.screenWidth / fontWidth;
        }

        public static string[] SplitLineToFitScreen(string message)
        {
            int maxLength = MaxTextLength();
            int necessaryLines = 1 + (message.Length / maxLength);
            string[] output = new string[necessaryLines];
            for (int i = 0; i < necessaryLines; i++)
            {
                int end = (int) MathF.Min(message.Length - (i * maxLength), maxLength);
                output[i] = message.Substring(i * maxLength, end);
            }

            return output;
        }

        // this outputs the loaded font as base64 and prints it
        // its hardcoded to work with the actual default font i use
        public static void OutputFont()
        {
            var fontBitArray = new BitArray(fontWidth * fontHeight * 16 * 8);
            for (int i = 0; i < fontWidth * 16; i++)
            for (int j = 0; j < fontHeight * 8; j++)
                fontBitArray[i + (j * fontWidth * 16)] = _fontBuffer[i, j];

            var fontBytes = new byte[fontBitArray.Length / 8];
            fontBitArray.CopyTo(fontBytes, 0);
            Console.WriteLine(Convert.ToBase64String(fontBytes));
        }

        private static bool LoadCachedFont(string fontPath)
        {
            if (!_fontCache.ContainsKey(fontPath)) return false;
            
            var cachedFont = _fontCache[fontPath];
            fontWidth = cachedFont.width;
            fontHeight = cachedFont.height;
            _fontBuffer = cachedFont.data;
            return true;
        }

        public static void LoadFont(string fontPath)
        {
            if (LoadCachedFont(fontPath)) return;

            var image = Raylib.LoadImage(fontPath);
            fontWidth = image.width;
            fontHeight = image.height;
            _fontBuffer = new bool[fontWidth, fontHeight];
            unsafe
            {
                Color32* colors = (Color32*) image.data;

                for (int i = 0; i < fontWidth; i++)
                {
                    for (int j = 0; j < fontHeight; j++)
                    {
                        int fontColorBufferIndex = (j * fontWidth) + i;
                        _fontBuffer[i, fontHeight - j - 1] = colors[fontColorBufferIndex].r > 0;
                    }
                }
            }
            Raylib.UnloadImage(image);

            fontWidth /= 16;
            fontHeight /= 8;
            _fontCache.Add(fontPath, (fontWidth, fontHeight, _fontBuffer));
        }

        public static void LoadDefaultFont()
        {
            if (LoadCachedFont("default")) return;

            var bytes = Convert.FromBase64String(DEFAULT_FONT_BASE64);
            var bitArray = new BitArray(bytes);
            fontWidth = 6;
            fontHeight = 8;
            _fontBuffer = new bool[16 * fontWidth, 8 * fontHeight];
            for (int i = 0; i < bitArray.Length; i++)
                _fontBuffer[i % (fontWidth * 16), i / (fontWidth * 16)] = bitArray[i];

            _fontCache.Add("default", (fontWidth, fontHeight, _fontBuffer));
        }

        public static void TextStyled(int x, int y, string text)
        {
            Color32 color = Colors.white;
            bool bold = false;
            bool wave = false;
            bool shadow = false;
            bool rainbow = false;
            int charOffset = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char nextChar = text[i];
                if (nextChar == '$')
                {
                    if (i == text.Length - 1) return;
                    char control = text[i + 1];
                    switch (control)
                    {
                        case 'c':
                            if (i == text.Length - 2) return;
                            char colorChar = text[i + 2];
                            switch (colorChar)
                            {
                                case '0':
                                    color = Colors.palette[0];
                                    break;
                                case '1':
                                    color = Colors.palette[1];
                                    break;
                                case '2':
                                    color = Colors.palette[2];
                                    break;
                                case '3':
                                    color = Colors.palette[3];
                                    break;
                                case '4':
                                    color = Colors.palette[4];
                                    break;
                                case '5':
                                    color = Colors.palette[5];
                                    break;
                                case '6':
                                    color = Colors.palette[6];
                                    break;
                                case '7':
                                    color = Colors.palette[7];
                                    break;
                                case '8':
                                    color = Colors.palette[8];
                                    break;
                                case '9':
                                    color = Colors.palette[9];
                                    break;
                                case 'A':
                                    color = Colors.palette[10];
                                    break;
                                case 'B':
                                    color = Colors.palette[11];
                                    break;
                                case 'C':
                                    color = Colors.palette[12];
                                    break;
                                case 'D':
                                    color = Colors.palette[13];
                                    break;
                                case 'E':
                                    color = Colors.palette[14];
                                    break;
                                case 'F':
                                    color = Colors.palette[15];
                                    break;
                            }

                            i += 2;
                            charOffset -= 3;
                            break;
                        case 'r':
                            rainbow = true;
                            i += 1;
                            charOffset -= 2;
                            break;
                        case 'b':
                            bold = true;
                            i += 1;
                            charOffset -= 2;
                            break;
                        case 'w':
                            wave = true;
                            i += 1;
                            charOffset -= 2;
                            break;
                        case 's':
                            shadow = true;
                            i += 1;
                            charOffset -= 2;
                            break;
                        case 'n':
                            bold = false;
                            wave = false;
                            shadow = false;
                            rainbow = false;
                            i += 1;
                            charOffset -= 2;
                            break;
                    }
                }
                else
                {
                    int yPos = y;
                    if (wave)
                    {
                        yPos += (int) (Math.Sin((-i + charOffset) + DisasterAPI.Engine.GetTime() * 6f) * 2f);
                    }

                    if (shadow)
                    {
                        Character(x + ((i + charOffset) * fontWidth), yPos + 1, text[i], Colors.black);
                        if (bold)
                        {
                            Character(1 + x + ((i + charOffset) * fontWidth), yPos + 1, text[i], Colors.black);
                        }
                    }

                    var tcol = color;
                    if (rainbow)
                    {
                        double colorIndex = Math.Floor(i + DisasterAPI.Engine.GetTime() * 8f);
                        colorIndex = ((colorIndex % RainbowColors.Length) + RainbowColors.Length) % RainbowColors.Length;
                        //Console.WriteLine(colorIndex);
                        color = RainbowColors[(int) colorIndex];
                    }

                    Character(x + ((i + charOffset) * fontWidth), yPos, text[i], color);
                    if (bold)
                    {
                        Character(1 + x + ((i + charOffset) * fontWidth), yPos, text[i], color);
                    }

                    color = tcol;
                }
            }
        }

        public static void Text(int x, int y, Color32 color, string text)
        {
            for (int i = 0; i < text.Length; i++)
                Character(x + (i * fontWidth), y, text[i], color);
        }

        public static void Paragraph(int x, int y, Color32 color, string text, int maxWidth = -1)
        {
            string[] lines = text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            int line = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (maxWidth != -1)
                {
                    for (int j = 0; j < lines[i].Length; j += maxWidth)
                    {
                        Text(x, y + (line * fontHeight), color,
                            lines[i].Substring(j, Math.Min(lines[i].Length - j, maxWidth)));
                        line += 1;
                    }
                }
                else
                {
                    Text(x, y + (i * fontHeight), color, lines[i]);
                }

                line += 1;
            }
        }

        private static void Character(int x, int y, int character, Color32 color)
        {
            x += ScreenController.offset.x;
            y += ScreenController.offset.y;
            
            int charX = (character % 16) * fontWidth;
            int charY = (int) MathF.Floor(character / 16);
            charY = 8 - charY - 1;
            charY *= fontHeight;
            for (int i = 0; i < fontWidth; i++)
            {
                for (int j = 0; j < fontHeight; j++)
                {
                    if (!_fontBuffer[charX + i, (charY + fontHeight) - j - 1]) continue;

                    var px = x + i;
                    var py = y + j;

                    void RenderAction()
                    {
                        Raylib.DrawPixel(px, py, color);
                    }

                    if (BufferRenderer.inBuffer)
                        BufferRenderer.Enqueue(RenderAction);
                    else
                        ShapeRenderer.EnqueueRender(RenderAction);
                }
            }
        }
    }
}