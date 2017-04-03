Shader "M8/ProBuilder/Unlit Solid Vertex Color" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }
	Lighting Off Fog { Mode Off }
    ColorMask RGB

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		fixed4 _Color;

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
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = v.color;
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = _Color * i.color;
			return col;
		}

		ENDCG
	}
  }
  Fallback "Diffuse"
}
