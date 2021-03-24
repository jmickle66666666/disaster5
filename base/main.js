var otherScript = load("test.js");
load("circle.js");
log(otherScript.value);

var color = {r:255, g: 127, b:20, a:255};

function update()
{
    Draw.clear();
    Draw.text(5, 5, "abcdefghijklmnopqrstuvwxyz!", color);
    Draw.text(5, 14, "ABCDEFGHIJKLMNOPQRSTUVWXYZ?", color);
}
// prints "test value"

