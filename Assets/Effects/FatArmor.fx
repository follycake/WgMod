#define REF_W 360
#define REF_H 224

sampler uImage0 : register(s0); // UV texture
sampler uImage1 : register(s1); // Armor texture

float2 uImageSize1;
float2 uOffset;
float4 uGlowColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
	float4 tex = tex2D(uImage0, coords);
	if (tex.a < 0.5f)
		return float4(0.0f, 0.0f, 0.0f, 0.0f);

	float2 coord = (tex.xy + uOffset) * float2(REF_W, REF_H);
	float4 col = tex2D(uImage1, coord / uImageSize1);
	col.rgb *= 1.0f - tex.b;
	col *= color;

	if (uImageSize1.y > REF_H)
	{
		coord.y += REF_H;
		float4 bright = tex2D(uImage1, coord / uImageSize1) * uGlowColor;
		col = lerp(col, bright, bright.a);
	}
	return col;
}

technique Technique1
{
	pass MainPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
