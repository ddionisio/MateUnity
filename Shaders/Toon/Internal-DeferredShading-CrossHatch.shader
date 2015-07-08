Shader "Hidden/Internal-DeferredShading-CrossHatch" {
Properties {
    _CrossHatchDeferredTexture("Cross Hatch Lookup", 2D) = "" {}
    _CrossHatchScale("Cross Hatch Scale", Float) = 1
    _CrossHatchTriplanarSharpness("Cross Hatch Tri-Planar Sharpness", Float) = 1

	_LightTexture0 ("", any) = "" {}
	_LightTextureB0 ("", 2D) = "" {}
	_ShadowMapTexture ("", any) = "" {}
	_SrcBlend ("", Float) = 1
	_DstBlend ("", Float) = 1
}
SubShader {

// Pass 1: Lighting pass
//  LDR case - Lighting encoded into a subtractive ARGB8 buffer
//  HDR case - Lighting additively blended into floating point buffer
Pass {
	ZWrite Off
	Blend [_SrcBlend] [_DstBlend]

CGPROGRAM
#pragma target 3.0
#pragma vertex vert_deferred_hatch
#pragma fragment frag
#pragma multi_compile_lightpass
#pragma multi_compile ___ UNITY_HDR_ON

#pragma exclude_renderers nomrt

#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityStandardBRDF.cginc"

sampler2D _CrossHatchDeferredTexture;
float _CrossHatchScale;
float _CrossHatchTriplanarSharpness;

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;

half4 CrossWeights(half shade) {
    half4 shadingFactor = half4(shade.xxxx);
    const half4 leftRoot = half4(-0.25, 0.0, 0.25, 0.5);
    const half4 rightRoot = half4(0.25, 0.5, 0.75, 1.0);

    return 4.0 * clamp(shadingFactor - leftRoot, 0, rightRoot - shadingFactor);
}

half4 CrossTex(float3 position, float3 normal) {
    float3 weights = pow(abs(normal), _CrossHatchTriplanarSharpness);
    weights /= (weights.x + weights.y + weights.z);

    half4 xTex = tex2D(_CrossHatchDeferredTexture, position.yz*_CrossHatchScale);
    half4 yTex = tex2D(_CrossHatchDeferredTexture, position.xz*_CrossHatchScale);
    half4 zTex = tex2D(_CrossHatchDeferredTexture, position.xy*_CrossHatchScale);

    return xTex*weights.x + yTex*weights.y + zTex*weights.z;
}

half CrossShade(half4 hatch, half4 weights) {
    half t = saturate(weights.x+weights.y+weights.z+weights.w);

    half shade = lerp(1, dot(weights, hatch.abgr), t);

    return shade;
}
		
half4 CalculateLight (unity_v2f_deferred i)
{
	float3 wpos;
	float2 uv;
	float atten, fadeDist;
	UnityLight light;
	UNITY_INITIALIZE_OUTPUT(UnityLight, light);

	UnityDeferredCalculateLightParams (i, wpos, uv, light.dir, atten, fadeDist);

	half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
	half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
	half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);

	light.color = _LightColor.rgb * atten;
	half3 baseColor = gbuffer0.rgb;
	half3 specColor = gbuffer1.rgb;
	half oneMinusRoughness = gbuffer1.a;
	half3 normalWorld = gbuffer2.rgb * 2 - 1;
	normalWorld = normalize(normalWorld);
	float3 eyeVec = normalize(wpos-_WorldSpaceCameraPos);
	half oneMinusReflectivity = 1 - SpecularStrength(specColor.rgb);
	light.ndotl = LambertTerm (normalWorld, light.dir);

	UnityIndirect ind;
	UNITY_INITIALIZE_OUTPUT(UnityIndirect, ind);
	ind.diffuse = 0;
	ind.specular = 0;

    //half4 res = UNITY_BRDF_PBS (baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normalWorld, -eyeVec, light, ind);

    //assuming a white surface to grab 'luminance'
    half4 lum = UNITY_BRDF_PBS (half3(1,1,1), 0, oneMinusReflectivity, oneMinusRoughness, normalWorld, -eyeVec, light, ind);

    half lumV = Luminance(lum);

    half4 crossW = CrossWeights(lumV);

    half4 crossInfo = CrossTex(wpos, normalWorld);

    half shade = CrossShade(crossInfo, crossW);

    half4 res = half4(shade.xxx, 1);

	return res;
}

unity_v2f_deferred vert_deferred_hatch (float4 vertex : POSITION, float3 normal : NORMAL)
{
	unity_v2f_deferred o;
	o.pos = mul(UNITY_MATRIX_MVP, vertex);
	o.uv = ComputeScreenPos (o.pos);
	o.ray = mul (UNITY_MATRIX_MV, vertex).xyz * float3(-1,-1,1);
	
	// normal contains a ray pointing from the camera to one of near plane's
	// corners in camera space when we are drawing a full screen quad.
	// Otherwise, when rendering 3D shapes, use the ray calculated here.
	o.ray = lerp(o.ray, normal, _LightAsQuad);

	return o;
}

#ifdef UNITY_HDR_ON
half4
#else
fixed4
#endif
frag (unity_v2f_deferred i) : SV_Target
{
	half4 c = CalculateLight(i);
	#ifdef UNITY_HDR_ON
	return c;
	#else
	return exp2(-c);
	#endif
}

ENDCG
}


// Pass 2: Final decode pass.
// Used only with HDR off, to decode the logarithmic buffer into the main RT
Pass {
	ZTest Always Cull Off ZWrite Off
	Stencil {
		ref [_StencilNonBackground]
		readmask [_StencilNonBackground]
		// Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
		compback equal
		compfront equal
	}

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers nomrt

sampler2D _LightBuffer;
struct v2f {
	float4 vertex : SV_POSITION;
	float2 texcoord : TEXCOORD0;
};

v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, vertex);
	o.texcoord = texcoord.xy;
	return o;
}

fixed4 frag (v2f i) : SV_Target
{
	return -log2(tex2D(_LightBuffer, i.texcoord));
}
ENDCG 
}

}
Fallback Off
}
