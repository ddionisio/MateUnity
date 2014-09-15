//only for 2D stuff
Shader "M8/Unlit/TextureAdditiveScroll" 
{
	Properties 
	{
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		speedX ("Speed X", Float) = 1
		speedY ("Speed Y", Float) = 1
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha One
		LOD 110
		
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			
			#pragma vertex vtx
			#pragma fragment frag 
			#pragma fragmentoption ARB_precision_hint_fastest
			
			struct Input {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct Output {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			sampler2D _MainTex;

			float4 _Color;
			
			float speedX;
			float speedY;
			
            float4 _MainTex_ST;
            			
			Output vtx(Input IN)
			{
				Output o;
				
				o.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				
				o.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				
				o.texcoord.x += speedX * _Time.y;
				o.texcoord.y += speedY * _Time.y;
				
				return o;
			}
			
			fixed4 frag(Output i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
				return col;
			}
			
			ENDCG
		}
	}
}
