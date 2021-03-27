var color = {r:255, g: 127, b:20, a:255};

Draw.loadFont("res/font1b.png");

var t = 0;
function update()
{
    t += System.deltaTime;
    Draw.clear();
    Draw.text(5, 5, "abcdefghijklmnopqrstuvwxyz!", color);
    Draw.text(5, 14, "ABCDEFGHIJKLMNOPQRSTUVWXYZ?", color);
    Draw.model(
        {
            x: 0, y: -1, z: -5
        },
        {
            x: 0, y: t / 1000, z: 0
        },
        "laptop.obj", "laptop.png"
    );
}

