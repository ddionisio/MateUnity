Shader "M8/Skybox/Blend" {
	Properties {  
		_Tint ("Tint Color", Color) = (.5, .5, .5, .5)  
		_Blend ("Blend", Range(0.0,1.0)) = 0.0  
		_FrontTex ("Primary Front (+Z)", 2D) = "white" {}  
		_BackTex ("Primary Back (-Z)", 2D) = "white" {}  
		_LeftTex ("Primary Left (+X)", 2D) = "white" {}  
		_RightTex ("Primary Right (-X)", 2D) = "white" {}  
		_UpTex ("Primary Up (+Y)", 2D) = "white" {}  
		_DownTex ("Primary Down (-Y)", 2D) = "white" {}  
		_FrontTex2("Secondary Front (+Z)", 2D) = "white" {}  
		_BackTex2("Secondary Back (-Z)", 2D) = "white" {}  
		_LeftTex2("Secondary Left (+X)", 2D) = "white" {}  
		_RightTex2("Secondary Right (-X)", 2D) = "white" {}  
		_UpTex2("Secondary Up (+Y)", 2D) = "white" {}  
		_DownTex2("Secondary Down (-Y)", 2D) = "white" {}  
	}
	
	SubShader {
		Tags { "Queue"="Background" "RenderType"="Background" }
		Cull Off ZWrite Off Fog { Mode Off }
		
		CGINCLUDE
		#include "UnityCG.cginc"

		fixed4 _Tint;
		fixed _Blend;
		
		struct appdata_t {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};
		struct v2f {
			float4 vertex : SV_POSITION;
			float2 texcoord : TEXCOORD0;
		};
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = v.texcoord;
			return o;
		}
		fixed4 skybox_frag (v2f i, sampler2D smp, sampler2D smp2)
		{
			fixed4 tex = tex2D (smp, i.texcoord);
			fixed4 tex2 = tex2D (smp2, i.texcoord);
			fixed4 col;
			col.rgb = lerp(tex.rgb, tex2.rgb, _Blend) + _Tint.rgb - unity_ColorSpaceGrey;
			col.a = tex.a * _Tint.a;
			return col;
		}
		ENDCG
	
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _FrontTex;
			sampler2D _FrontTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex2); }
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _BackTex;
			sampler2D _BackTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex2); }
			ENDCG
		}
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _LeftTex;
			sampler2D _LeftTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex2); }
			ENDCG
		}
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _RightTex;
			sampler2D _RightTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex2); }
			ENDCG
		}
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _UpTex;
			sampler2D _UpTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex2); }
			ENDCG
		}
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			sampler2D _DownTex;
			sampler2D _DownTex2;
			fixed4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex2); }
			ENDCG
		}
	}
	
	Fallback "RenderFX/Skybox", 1
}