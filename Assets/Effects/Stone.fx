sampler uImage0 : register(s0); // Texture
sampler uImage1 : register(s1); // Stone texture

float uOpacity;
float2 uImageSize1;
matrix uMatrix;

struct VertexShaderInput
{
    float4 position : POSITION0;
    float4 color : COLOR0;
    float2 coords : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 position : SV_POSITION;
    float4 color : COLOR0;
    float2 coords : TEXCOORD0;
	float2 patternCoords : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.position = mul(input.position, uMatrix);
    output.color = input.color;
    output.coords = input.coords;
	output.patternCoords = input.position;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 col = tex2D(uImage0, input.coords) * input.color;
	col.rgb *= lerp(1.0f, tex2D(uImage1, (floor(input.patternCoords) + 0.5f) / uImageSize1), uOpacity);
	return col;
}

technique Technique1
{
	pass MainPass
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
