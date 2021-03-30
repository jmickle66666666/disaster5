using System;
using System.Numerics;
using DisasterEngine;

public class Testy
{
    Color32 color = new Color32(255, 127, 20, 255);
    Color32 color2 = new Color32(255, 127, 20, 255);
    float[] rand = new float[] { 0.72f, 0.04f, 0.8f, 0.55f, 0.56f, 0.2f, 0.86f, 0.84f, 0.16f, 0.9f, 0.38f, 0.87f, 0.74f, 0.45f, 0.36f, 0.24f, 0.94f, 0.25f, 0.12f, 0.31f, 0.12f, 0.88f, 0.68f, 0.59f, 0.4f, 0.76f, 0.59f, 0.85f, 0.56f, 0.72f, 0.24f, 0.49f, 0.3f, 0.15f, 0.17f, 0.63f, 0.96f, 0.57f, 0.61f, 0.38f, 0.12f, 0.57f, 0.8f, 0.01f, 0.88f, 0.34f, 0.6f, 0.93f, 0.47f, 0.34f, 0.67f, 0.93f, 0.54f, 0.05f, 0.77f, 0.67f, 0.55f, 0.71f, 0.58f, 0.9f, 0.55f, 0.49f, 0.05f, 0.59f };
    int randIndex = 0;

    Vector3 position = new Vector3(0, -1, -5);
    Vector3 rotation = new Vector3(0, 0, 0);

    float t = 0;

    public void Init()
    {
        Console.WriteLine("Hello!!");
        Draw.LoadFont("font1b.png");
    }

    public float Random()
    {
        var output = rand[randIndex];
        randIndex += 1;
        if (randIndex >= rand.Length)
        {
            randIndex = 0;
        }
        return output;
    }

    public void Update(float deltaTime)
    {

        t += deltaTime;
        Draw.Clear();
        Draw.Text(5, 5, "spinning laptop zome v2!", color);

        float ct = t / 100f;
        color2.b = (byte)(int)MathF.Floor(128 + MathF.Sin(ct) * 128);
        color2.r = (byte)(int)MathF.Floor(128 + MathF.Sin(ct + 2) * 128);
        color2.g = (byte)(int)MathF.Floor(128 + MathF.Sin(ct + 4) * 128);
        Draw.Text(75, 124, "SPINNING LAPTOP ZOME", color2);
        randIndex = 0;
        for (int i = 0; i < 10000; i++)
        {
            rotation.Y = t / 1000f;
            position.X = -5 + Random() * 10f;
            position.Y = -4 + Random() * 6f;
            position.Z = -5 - Random() * 25f;
            Draw.Model(
                position, rotation,
                "laptop.obj", "laptop.png"
            );
        }
    }
}