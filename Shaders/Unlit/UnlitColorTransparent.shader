Shader "M8/Unlit/Color Transparent" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader {
      Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
      LOD 100

      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      //Blend SrcAlpha One
      ColorMask RGB
      Lighting Off Fog{ Mode Off }

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
		};

		struct v2f_vct
		{
			float4 vertex : POSITION;
		};

		v2f_vct vert_vct(vin_vct v)
		{
			v2f_vct o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR {
			return _Color;
		}

		ENDCG
	}
  }
}
