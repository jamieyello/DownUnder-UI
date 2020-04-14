// HLSL

#if OPENGL
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
float ShadeVisibility = 0.8;

// Border
float BorderWidth = 70;
float BorderExponential = 2;
float BorderVisibility = 1;

// Gradient
float2 GradientVisibility = float2(0.2, 0.4);
float2 GradientExponential = float2(1, 1);

// ---

// Methods ---
float1 VerticalGradient(float y, float height)
{
    return y / height;
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
    
    float left = Size.x * input.TextureCoordinates.x;
    float top = Size.y * input.TextureCoordinates.y;
    float bottom = Size.y - top;
    float right = Size.x - left;
    float distance = min(min(top, bottom), min(left, right));
    float shade = distance / BorderWidth;
    shade = 1 - 
    (
        pow(1 - min(shade, 1), BorderExponential) * BorderVisibility
        + pow(input.TextureCoordinates.x, GradientExponential.x) * GradientVisibility.x
        + pow(input.TextureCoordinates.y, GradientExponential.y) * GradientVisibility.y
    ) * ShadeVisibility;
    
    color.rgb *= shade;
    return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};



