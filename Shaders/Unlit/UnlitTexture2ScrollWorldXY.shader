// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "M8/Unlit/Texture 2 Scroll World XY" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_MainTex2 ("Texture2", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
    scale ("Scale", Float) = 1
	
    speedX2("Speed 2 X", Float) = 1
    speedY2("Speed 2 Y", Float) = 1
    scale2("Scale 2", Float) = 1
	
	colorMod1 ("Color", Color) = (1,1,1,1)
	colorMod2 ("Color 2", Color) = (1,1,1,1)
  }
  SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
    LOD 100

    ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	//Blend SrcAlpha One
    ColorMask RGB
	Lighting Off Fog { Mode Off }

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
        sampler2D _MainTex2;
		
		float speedX;
		float speedY;
        float scale;

        float speedX2;
        float speedY2;
        float scale2;
		
		fixed4 colorMod1;
        fixed4 colorMod2;

		struct vin_vct 
		{
			float4 vertex : POSITION;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
            float2 texcoord2 : TEXCOORD1;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			float4 wVtx = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
		
			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_VP, wVtx);

			o.texcoord = wVtx.xy * scale;
			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;

            o.texcoord2 = wVtx.xy * scale2;
            o.texcoord2.x += speedX2 * _Time.y;
            o.texcoord2.y += speedY2 * _Time.y;

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
            fixed4 clr = tex2D(_MainTex, i.texcoord) * colorMod1;
            clr += tex2D(_MainTex2, i.texcoord2) * colorMod2;
			return clr;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
