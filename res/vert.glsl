uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

in vec3 pos;
in vec3 normal;
in vec2 uv;

out varying vec2 fragUV;
out varying float dist;

void main(void)
{
  gl_Position = projection_matrix * modelview_matrix * vec4(pos, 1);
  dist = gl_Position.z;
  fragUV = uv;
}