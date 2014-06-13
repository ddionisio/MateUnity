Shader "Hidden/TransCrossFade" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _SourceTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"

		struct v2f {
			 half4 pos : POSITION;
			 half2 uv : TEXCOORD0;
		 };
		
		sampler2D _MainTex;
        sampler2D _SourceTex;
		fixed _t;
				
		half4 frag(v2f i) : COLOR {
			return lerp(tex2D(_MainTex, i.uv), tex2D(_SourceTex, i.uv), _t);
		}

		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
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
