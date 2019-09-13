Shader "M8/Unlit/Vertex Color Additive Texture Scroll" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
	_OverlayTex("Base (RGB) Trans (A)", 2D) = "white" {}
	
	speedX("Speed X", Float) = 1
	speedY("Speed Y", Float) = 1
  }
  SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	ZWrite Off 
	Lighting Off Fog { Mode Off }
    ColorMask RGB
    Blend SrcAlpha One

	Pass {
        Name "FORWARD"

		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		fixed4 _Color;

		sampler2D _OverlayTex;
		float4 _OverlayTex_ST;

		float speedX;
		float speedY;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color * _Color;

			o.texcoord = TRANSFORM_TEX(v.texcoord, _OverlayTex);

			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR {
			fixed4 clr = tex2D(_OverlayTex, i.texcoord) * i.color;
			clr *= clr.a;

			return clr;
		}

		ENDCG
	}
  }
}
