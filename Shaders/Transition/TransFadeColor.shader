Shader "Hidden/TransFadeColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"

		struct v2f {
			 half4 pos : POSITION;
			 half2 uv : TEXCOORD0;
		 };
		
		sampler2D _MainTex;
		half4 _Color;
		fixed _t;
				
		half4 frag(v2f i) : COLOR {
			return lerp(tex2D(_MainTex, i.uv), _Color, _t);
		}

		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = v.texcoord.xy;
			return o;
		}
	ENDCG

	Subshader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
