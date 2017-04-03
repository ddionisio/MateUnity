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
			 half2 uv1 : TEXCOORD1;
		 };
		
		sampler2D _MainTex;
        sampler2D _SourceTex;
		uniform float4 _MainTex_TexelSize;
		fixed _t;
				
		half4 frag(v2f i) : COLOR {
			return lerp(tex2D(_MainTex, i.uv), tex2D(_SourceTex, i.uv1), _t);
		}

		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = v.texcoord.xy;
			
			o.uv1 = v.texcoord.xy;
			 
			#if SHADER_API_D3D9 || SHADER_API_XBOX360 || SHADER_API_D3D11
			if (_MainTex_TexelSize.y < 0)
				o.uv1.y = 1-o.uv1.y;
			#endif
			
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
