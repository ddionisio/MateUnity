Shader "M8/ProBuilder/Unlit Vertex Color Mod Z Color" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	_Mod ("Mod", Color) = (1,1,1,1)
	_ZColor ("Tint Z", Color) = (0.5,0.5,0.5,0.3)
	_ZOfs ("Z Offset", Float) = 0
	_ZWorld("Z World", Float) = 0
	_ZUnit("Z Measurement", Float) = 0.5
  }
  SubShader {
    Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
	Lighting Off Fog { Mode Off }
    Blend One OneMinusSrcAlpha

	Pass {
		CGPROGRAM
		#pragma vertex vert_vct
		#pragma fragment frag_mult
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Mod;

		fixed4 _ZColor;
		float _ZWorld;         //make sure to set this during startup
		float _ZOfs;		   //this is the focused world z
		float _ZUnit;

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
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color * _Mod;
			o.texcoord = v.texcoord;

			o.color *= lerp(fixed4(1,1,1,1), _ZColor, clamp(abs(_ZWorld - _ZOfs)/_ZUnit, 0, 1));

			return o;
		}

		fixed4 frag_mult(v2f_vct i) : COLOR
		{
			fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
			return col;
		}

		ENDCG
	}
  }
  Fallback "Diffuse"
}
