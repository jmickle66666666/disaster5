in vec3 pos;
in vec2 uv;
out varying vec2 fuv;

void main(void)
{
    fuv = uv;
    gl_Position = vec4(pos,1);
}