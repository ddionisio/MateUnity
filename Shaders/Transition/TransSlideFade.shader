Shader "Hidden/TransSlideFade" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _SourceTex ("Base (RGB)", 2D) = "white" {}
		_AlphaMaskTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
		#include "UnityCG.cginc"

		struct v2f {
			 half4 pos : POSITION;
			 half2 uv : TEXCOORD0;
		 };
		
		sampler2D _MainTex;
        sampler2D _SourceTex;
		sampler2D _AlphaMaskTex;
		uniform float2 _Scroll; //[xy: scroll]
		fixed _t;

		half4 frag(v2f i) : COLOR {
			half2 alphaUV = i.uv - _Scroll;
			half4 alpha = tex2D(_AlphaMaskTex, alphaUV);
			fixed alphaT = alpha.r * _t;
			
			return lerp(tex2D(_MainTex, i.uv), tex2D(_SourceTex, i.uv), alphaT);
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