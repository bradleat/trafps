float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

float4 xlightColor;
float3 xlightDirection;
float4 xambientColor;
Texture xTexture;

sampler TextureSampler = sampler_state
{
	texture = <xTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = clamp;
	AddressV = clamp;
};

struct VertexToPixel
{
	float4 Position	: POSITION0;
	float4 Color	: COLOR0;
	float2 TexCoords: TEXCOORD0;
};

struct PixelToFrame
{
	float4 Color	: COLOR0;
};

VertexToPixel VertexShaderFunction(float3 Position : POSITION,
	float3 Normal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel output = (VertexToPixel)0;

	float4x4 worldViewProjection = mul(mul(xWorld, xView), xProjection);
	float3 worldNormal = mul(Normal, xWorld);
	
	float diffuseIntensity = saturate(dot(-xlightDirection, worldNormal));
	float4 diffuseColor = xlightColor * diffuseIntensity;
	
    output.Position = mul(float4(Position, 1.0), worldViewProjection);
    output.Color = diffuseColor + xambientColor;
    output.TexCoords = inTexCoords;
    
    return output;
}

PixelToFrame PixelShaderFunction(VertexToPixel PSIn)
{
    PixelToFrame output = (PixelToFrame)0;
    
	float4 baseColor= tex2D(TextureSampler, PSIn.TexCoords);
	output.Color = baseColor * PSIn.Color;
      
    return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_1_1 PixelShaderFunction();
    }
}
