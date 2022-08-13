bool _CustomShadowEnable = false;
Texture2D _CustomShadowMap;
float4x4 _CustomShadowMap_View;
float4x4 _CustomShadowMap_Proj;
float _CustomShadow_Bias;

SamplerState my_point_clamp_sampler;

float checkCustomShadow(float3 positionWS, float originalShadowAttenuation)
{
	if (_CustomShadowEnable == false) { return originalShadowAttenuation; }

	float4 positionVS_S = mul(_CustomShadowMap_View, float4(positionWS, 1));
	float4 positionCS_S = mul(_CustomShadowMap_Proj, positionVS_S);
	
	float2 uv = lerp(positionCS_S.xy / positionCS_S.w, float2(1, 1), 0.5f);
	if (uv.x < 0 || 1 < uv.x || uv.y < 0 || 1 < uv.y) { return 1; }
	#if UNITY_UV_STARTS_AT_TOP
		uv.y = 1 - uv.y;
	#endif

	float shadowMapDepth = _CustomShadowMap.Sample(my_point_clamp_sampler, uv).x;
	float surfaceDepth = -positionVS_S.z;

	if (shadowMapDepth + _CustomShadow_Bias < surfaceDepth) { return 0; }
	else { return 1; }
}