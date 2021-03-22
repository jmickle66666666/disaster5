uniform vec3 color;
uniform sampler2D texture;
in vec2 fragUV;
in float dist;

void main(void)
{
  // doing a tiny big of like. distance based fog
  dist = 1/dist;
  dist = 1.0 - dist;
  dist *= dist;
  dist = 1.0 - dist;
  dist *= 2;
  // gl_FragColor = vec4(dist, dist, dist, 1.0);
  gl_FragColor = texture2D(texture, fragUV) * dist;
}