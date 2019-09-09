Shader "M8/Unlit/Vertex Color Transparent Texture Additive Scroll World XZ" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)

	_OverlayTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_OverlayColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	speedX("Speed X", Float) = 1
	speedY("Speed Y", Float) = 1
  }
  SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off Fog { Mode Off }
    ColorMask RGB
    Blend SrcAlpha OneMinusSrcAlpha

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

		fixed4 _OverlayColor;

		float speedX;
		float speedY;

		struct vin_vct 
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			float4 wVtx = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));

			v2f_vct o;
			o.vertex = mul(UNITY_MATRIX_VP, wVtx);
			o.color = v.color * _Color;

			o.texcoord = TRANSFORM_TEX(wVtx.xz, _OverlayTex);

			o.texcoord.x += speedX * _Time.y;
			o.texcoord.y += speedY * _Time.y;

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR {

			fixed4 overlayCol = tex2D(_OverlayTex, i.texcoord) * _OverlayColor;
			overlayCol *= overlayCol.a;

			fixed4 clr = fixed4(clamp(i.color.rgb + overlayCol.rgb, 0, 1), i.color.a);

			return clr;
		}

		ENDCG
	}
  }
}
