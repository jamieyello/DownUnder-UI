#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float2 Origin;
float2 Size;

Texture2D TextureA;
sampler2D TextureASampler = sampler_state
{
	Texture = <TextureA>;
};

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//return float4(1,1,1,1);
	//float4 result = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	//result.a = 1;
	
	float4 result = float4(0,0,0,0);
	
	for (int i = 0; i < 8; i++)
	{
		result = result + tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0005 * (i - 4)));
	}
	
	return result / 8;
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};