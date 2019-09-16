// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "M8/Unlit/Texture Vertex Color Transparent" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	
	_Color ("Color", Color) = (1,1,1,1)
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
		
		fixed4 _Color;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			fixed4 color : COLOR;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
			fixed4 color : COLOR;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.color = v.color * _Color;

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
            fixed4 clr = tex2D(_MainTex, i.texcoord) * i.color;
			return clr * clr.a;
		}

		ENDCG
	}
  }
  Fallback "Transparent/Diffuse"
}
