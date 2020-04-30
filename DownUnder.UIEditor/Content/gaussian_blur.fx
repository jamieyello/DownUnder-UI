#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// http://rastergrid.com/blog/2010/09/efficient-gaussian-blur-with-linear-sampling/

// Parameters ---
Texture2D SpriteTexture;
sampler s0;
float2 Size = float2(1, 1);

uniform float offset[3] = { 0.0, 1.3846153846, 3.2307692308 };
uniform float weight[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = input.Color;
    //float2 pos = input.TextureCoordinates;
    
    //for (int i = 1; i < 3; i++)
    //{
    //    float4 s = tex2D(s0, float2(0, 0));
        
    //    color += tex2D(s0, (pos + float2(0.0, offset[i])) / 1024.0) * weight[i];
    //    color += tex2D(s0, (pos - float2(0.0, offset[i])) / 1024.0) * weight[i];
    //}
    return color;
    //+float4(0, 0, 0, 1);
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
