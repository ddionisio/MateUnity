Shader "M8/Unlit/Texture World XY Wave Multiplicative" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
	
	amplitudeX ("amplitude X", Float) = 0.017453292
	amplitudeY ("amplitude Y", Float) = 0.017453292
	
	rangeX ("range X", Float) = 0.017453292
	rangeY ("range Y", Float) = 0.017453292
	
	colorMod ("Color", Color) = (1,1,1,1)
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
    LOD 100

    ZWrite Off
	Blend Zero SrcColor
    ColorMask RGB
	Lighting Off Fog { Mode Off }

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		float speedX;
		float speedY;
		
		float amplitudeX;
		float amplitudeY;
		
		float rangeX;
		float rangeY;
		
		fixed4 colorMod;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			float4 wVtx = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
		
			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_VP, wVtx);
			
			o.texcoord = TRANSFORM_TEX(wVtx.xy, _MainTex);

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
		    float2 texCoord = float2(
				i.texcoord.x + sin(i.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
				i.texcoord.y + sin(i.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
			
            fixed4 clr = tex2D(_MainTex, texCoord) * colorMod;
			
			return lerp(1, clr, clr.a);
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
