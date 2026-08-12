sampler uImage0 : register(s0);

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 col = tex2D(uImage0, coords);
    col.rgb = (col.rgb - 0.032f) / (1.0f - 0.032f);
	if (all(col.rgb < 0.2f))
		col.rgb = 0.5f;
	return col;
}

technique Technique1
{
	pass MainPass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
