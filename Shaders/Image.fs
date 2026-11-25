uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;
#define EPS  .01
#define COL0 vec3(.2, .35, .55)
#define COL1 vec3(.9, .43, .34)
#define COL2 vec3(.96, .66, .13)
#define COL3 vec3(0.0)
#define COL4 vec3(0.99,0.1,0.09)

/*
 * Interação — Distância ao Centro (Baricentro) do Triângulo
 *
 * 1. O shader calcula as coordenadas baricêntricas (u, v, w) em bary() para
 *    verificar em tempo real se o cursor do mouse (mc) está dentro do triângulo ABC.
 * 2. Ao passar o mouse sobre o triângulo (sem precisar clicar), o baricentro é
 *    calculado como center = (a + b + c) / 3.0 e a distância euclidiana como
 *    dist = length(mc - center).
 * 3. A distância é exibida visualmente de quatro formas:
 *    - Ponto central marcando o baricentro do triângulo.
 *    - Segmento de reta ligando o cursor do mouse ao baricentro.
 *    - Círculo centrado no baricentro com raio igual à distância atual.
 *    - Intensidade de cor no interior do triângulo proporcional à distância.
 */


/*  Install  Istructions

sudo apt-get install g++ cmake git
 sudo apt-get install libsoil-dev libglm-dev libassimp-dev libglew-dev libglfw3-dev libxinerama-dev libxcursor-dev
libxi-dev libfreetype-dev libgl1-mesa-dev xorg-dev

git clone https://github.com/JoeyDeVries/LearnOpenGL.git*/

float df_circ(in vec2 p, in vec2 c, in float r)
{
    return abs(r - length(p - c));
}

// Find the intersection of "p" onto "ab".
vec2 intersect (vec2 p, vec2 a, vec2 b)
{
    // Calculate the unit vector from "a" to "b".
    vec2 ba = normalize(b - a);

    // Calculate the intersection of p onto "ab" by
    // calculating the dot product between the unit vector
    // "ba" and the direction vector from "a" to "p", then
    // this value is multiplied by the unit vector "ab"
    // fired from the point "a".
    return a + ba * dot(ba, p - a);
}


// Visual line for debugging purposes.
bool line (vec2 p, vec2 a, vec2 b)
{
    // Direction from a to b.
    vec2 ab = normalize(b - a);

    // Direction from a to the pixel.
    vec2 ap = p - a;

    // Find the intersection of the pixel on to vector
    // from a to b, calculate the distance between the
    // pixel and the intersection point, then compare
    // that distance to the line width.
    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

float df_line(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
        float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);
        return length(pa - ba * h);
}

float sharpen(in float d, in float w)
{
    float e = 1. / min(iResolution.y , iResolution.x);
    return 1. - smoothstep(-e, e, d - w);
}

vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p)
{
    vec3 v0 = b - a, v1 = c - a, v2 = p - a;
    float d00 = dot(v0, v0);
    float d01 = dot(v0, v1);
    float d11 = dot(v1, v1);
    float d20 = dot(v2, v0);
    float d21 = dot(v2, v1);
    float denom = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / denom;
    float w = (d00 * d21 - d01 * d20) / denom;
    float u = 1.0 - v - w;
    return vec3(u, v, w);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords)
{
    barycoords = bary(vec3(a.x, 0., a.y),
                  vec3(b.x, 0., b.y),
                  vec3(c.x, 0., c.y),
                  vec3(p.x, 0., p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}


float df_bounds(in vec2 uv, in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec3 barycoords)
{
    float cp = 0.;





    return cp;
}

/*vec3 scene(in vec2 uv, in vec2 a, in vec2 b, in vec2 c, in vec2 p)
{
    float d = df_bounds(uv, p, a, b, c);
    return d > 0. ? COL3 : COL1;
}*/

vec3 globalColor (in vec2 uv, in vec2 a, in vec2 b, in vec2 c)
{
    vec3 r=vec3(1.0);

    return r;
}

void main()
{
    float ar = iResolution.x / iResolution.y;
    vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);
    vec2 mc = (iMouse.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);

    vec2 a = vec2( .73,  .75);
    vec2 b = vec2(-.85,  .15);
    vec2 c = vec2( .25, -.75);
    vec3 barycoords;

    bool t0 = test(a, b, c, mc, barycoords);
    float l = 0.1;

    bool t1 = test(a, b, c, uv, barycoords);
    vec3 r = globalColor(uv, a, b, c);
    bool testcc = t1;

    vec2 center = (a + b + c) / 3.0;
    float dist = length(mc - center);

    vec3 color = vec3(0.0);
    if (t0 && t1)
        color = vec3(dist * 0.6); // escurece o triângulo conforme afasta do centro

    // Visual debug lines and points.
    if (line(uv, a, b))
        color = vec3(1.0, 1.0, 0.0);
    if (line(uv, b, c))
        color = vec3(1.0, 0.0, 1.0);
    if (line(uv, c, a))
        color = vec3(0.0, 1.0, 1.0);
    if (df_circ(uv, a, EPS) < 0.5 * EPS)
        color = vec3(0.0, 1.0, 0.0);
    if (df_circ(uv, b, EPS) < 0.5 * EPS)
        color = vec3(1.0, 0.0, 0.0);
    if (df_circ(uv, c, EPS) < 0.5 * EPS)
        color = vec3(0.0, 0.0, 1.0);

    // Exibe o centro, o segmento até o mouse e o círculo de raio = dist
    if (t0)
    {
        if (df_circ(uv, center, EPS) < 0.5 * EPS)
            color = vec3(1.0, 0.5, 0.0);
        if (dist > 0.0001 && df_line(uv, mc, center) < 0.005)
            color = vec3(0.0, 1.0, 1.0);
        if (df_circ(uv, center, dist) < 0.004)
            color = vec3(0.0, 1.0, 1.0);
    }

    vec3 col = l > 0. ? (testcc ? color : vec3(1) - color) : (t1 ? r : (t0 ? COL3 + color : COL2 - color));

    gl_FragColor = vec4(1.0 - col, 1);
}