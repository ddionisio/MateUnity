// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "M8/Unlit/Texture Scroll Transparent" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
	
	colorMod1 ("Color", Color) = (1,1,1,1)
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
        float4 _MainTex_ST;
		
		float speedX;
		float speedY;

		fixed4 colorMod1;

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
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
            fixed4 clr = tex2D(_MainTex, i.texcoord) * colorMod1;
			return clr;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
