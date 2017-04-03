Shader "Hidden/Internal-DeferredShading-CrossHatch" {
Properties {
    _CrossHatchDeferredTexture("Cross Hatch Lookup", 2D) = "" {}

    _CrossHatchDeferredLightTexture("Cross Hatch Light Lookup", 2D) = "" {}

    _CrossHatchDeferredLightRamp("Light Ramp", 2D) = "" {}

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

sampler2D _CrossHatchDeferredLightTexture;

sampler2D _CrossHatchDeferredLightRamp;

sampler2D _CameraGBufferTexture0;
sampler2D _CameraGBufferTexture1;
sampler2D _CameraGBufferTexture2;

//modified PBS with separate diffuse and specular output
void PBS_1_DiffuseSpec(half3 specColor, half oneMinusReflectivity, half oneMinusRoughness,
	half3 normal, half3 viewDir, UnityLight light, UnityIndirect gi,
    out half3 diffOut, out half3 specOut)
{
	half roughness = 1-oneMinusRoughness;
	half3 halfDir = Unity_SafeNormalize (light.dir + viewDir);

	half nl = light.ndotl;
	half nh = BlinnTerm (normal, halfDir);
	half nv = DotClamped (normal, viewDir);
	half lv = DotClamped (light.dir, viewDir);
	half lh = DotClamped (light.dir, halfDir);

#if UNITY_BRDF_GGX
	half V = SmithGGXVisibilityTerm (nl, nv, roughness);
	half D = GGXTerm (nh, roughness);
#else
	half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);
	half D = NDFBlinnPhongNormalizedTerm (nh, RoughnessToSpecPower (roughness));
#endif

	half nlPow5 = Pow5 (1-nl);
	half nvPow5 = Pow5 (1-nv);
	half Fd90 = 0.5 + 2 * lh * lh * roughness;
	half disneyDiffuse = (1 + (Fd90-1) * nlPow5) * (1 + (Fd90-1) * nvPow5);
	
	// HACK: theoretically we should divide by Pi diffuseTerm and not multiply specularTerm!
	// BUT 1) that will make shader look significantly darker than Legacy ones
	// and 2) on engine side "Non-important" lights have to be divided by Pi to in cases when they are injected into ambient SH
	// NOTE: multiplication by Pi is part of single constant together with 1/4 now

	half specularTerm = (V * D) * (UNITY_PI / 4);// Torrance-Sparrow model, Fresnel is applied later (for optimization reasons)
	if (IsGammaSpace())
		specularTerm = sqrt(max(1e-4h, specularTerm));

    half grazingTerm = saturate(oneMinusRoughness + (1-oneMinusReflectivity));
	
    diffOut = disneyDiffuse * nl;
    specOut = specularTerm * light.color * FresnelTerm(specColor, lh) + gi.specular * FresnelLerp (specColor, grazingTerm, nv);
}

float3 TriPlanarWeights(float3 normal) {
    const float _TriplanarSharpness = 1;

    float3 weights = pow(abs(normal), _TriplanarSharpness);
    weights /= (weights.x + weights.y + weights.z);

    return weights;
}

half4 Tex2DTriPlanar(sampler2D tex, float3 position, float3 weights, float scale) {
    half4 xTex = tex2D(tex, position.yz*scale);
    half4 yTex = tex2D(tex, position.xz*scale);
    half4 zTex = tex2D(tex, position.xy*scale);

    return xTex*weights.x + yTex*weights.y + zTex*weights.z;
}

half CrossShade(half shade, float3 position, half3 texWeights) {
    const float _CrossHatchScale = 1;

    //grab hatch info
    half4 hatch = Tex2DTriPlanar(_CrossHatchDeferredTexture, position, texWeights, _CrossHatchScale);

    //compute weights
    half4 shadingFactor = half4(shade.xxxx);
    const half4 leftRoot = half4(-0.25, 0.0, 0.25, 0.5);
    const half4 rightRoot = half4(0.25, 0.5, 0.75, 1.0);

    half4 weights = 4.0 * max(0, min(rightRoot - shadingFactor, shadingFactor - leftRoot));

    //final shade

    return dot(weights, hatch.abgr) + 4.0*clamp(shade - 0.75, 0, 0.25);
}

half CrossLight(half shade, float3 position, half3 texWeights) {
    const float _CrossHatchScale = 0.5;

    //grab hatch info
    half4 hatch = Tex2DTriPlanar(_CrossHatchDeferredLightTexture, position, texWeights, _CrossHatchScale);

    //compute weights
    half4 shadingFactor = half4(shade.xxxx);
    const half4 leftRoot = half4(-0.25, 0.0, 0.25, 0.5);
    const half4 rightRoot = half4(0.25, 0.5, 0.75, 1.0);

    half4 weights = 4.0 * max(0, min(rightRoot - shadingFactor, shadingFactor - leftRoot));

    //final shade

    return dot(weights, hatch.abgr) + 4.0*clamp(shade - 0.75, 0, 0.25);
}

/*half Displacement(half lum, float3 position, half3 texWeights) {
    const float _CrossHatchLightOffsetScale = 0.1;
    const float _CrossHatchLightOffsetAmount = -0.15;

    half4 ofs = Tex2DTriPlanar(_CrossHatchDeferredLightOffset, position, texWeights, _CrossHatchLightOffsetScale);
    return saturate(lum + ofs.x*_CrossHatchLightOffsetAmount);
}*/

half3 Posterize(half3 color) {
    const float _CrossHatchPosterizeGamma = 0.6;
    const float _CrossHatchPosterizeColors = 6; 

    half3 ret = pow(color, _CrossHatchPosterizeGamma);
    ret = floor(ret*_CrossHatchPosterizeColors) / _CrossHatchPosterizeColors;
   
    return pow(ret, 1.0/_CrossHatchPosterizeGamma);
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

    //compute light
    half3 diffuseTerm;
    half3 specular;

    PBS_1_DiffuseSpec(specColor, oneMinusReflectivity, oneMinusRoughness, normalWorld, -eyeVec, light, ind, diffuseTerm, specular);

    ////////////////

    //grab texture uv weights
    float3 uvWeights = TriPlanarWeights(normalWorld);

    //--lighting

    //offset diffuse
    half diffuseTermOfs = CrossLight(diffuseTerm.r, wpos, uvWeights); //Displacement(diffuseTerm.r, wpos, uvWeights);
        
    half3 l = ind.diffuse + light.color*tex2D(_CrossHatchDeferredLightRamp, half2(diffuseTermOfs, 0.5)); //lum*tex2D(_CrossHatchDeferredLightRamp, half2(lumVOfs, 0.5));
    //half3 l = ind.diffuse + light.color*diffuseTermOfs;

    //--cross-hatch
    half lumV = Luminance(light.color*diffuseTerm);
    half cross = CrossShade(lumV, wpos, uvWeights);

    //TODO: specular
    
    ////////////////

    half4 res = half4(baseColor*l*cross, 1);

	return res;
}

unity_v2f_deferred vert_deferred_hatch (float4 vertex : POSITION, float3 normal : NORMAL)
{
	unity_v2f_deferred o;
	o.pos = UnityObjectToClipPos(vertex);
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
	o.vertex = UnityObjectToClipPos(vertex);
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
