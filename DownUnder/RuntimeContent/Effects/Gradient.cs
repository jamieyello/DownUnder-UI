using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.RuntimeContent.Effects
{
    internal class Gradient : IEffectSource
    {
        public string FX { get => fx; }
        public const string fx = 
@"#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Parameters ---
Texture2D SpriteTexture;
sampler s0;
float2 Size = float2(1, 1);

// Shade
float ShadeVisibility = 0.5;
float4 ShadeColor = float4(0, 0, 0, 1);

// Border
float BorderWidth = 40;
float BorderExponential = 1;
float BorderVisibility = 1;

// Gradient
float2 GradientVisibility = float2(.5, .5);
float2 GradientExponential = float2(1, 1);

// ---

// Methods ---
float1 DistanceFromSide(float2 pos, float2 size)
{
    float left = size.x * pos.x;
    float top = size.y * pos.y;
    return min(min(top, size.y - top), min(left, size.x - left));
}

// pseudo random generator https://gist.github.com/keijiro/ee7bc388272548396870
float nrand(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};
// ---

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = input.Color;
    
    float shade = 
    (
        pow(1 - min(DistanceFromSide(input.TextureCoordinates, Size) / BorderWidth, 1), BorderExponential) * BorderVisibility // Border
        + pow(input.TextureCoordinates.x, GradientExponential.x) * GradientVisibility.x // X gradient
        + pow(input.TextureCoordinates.y, GradientExponential.y) * GradientVisibility.y // Y gradient
    ) * ShadeVisibility;
    
    color = 
        lerp(color, ShadeColor, shade) 
        + nrand(input.TextureCoordinates) / 255; // Noise dithering 
    
    return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
";
    }
}
