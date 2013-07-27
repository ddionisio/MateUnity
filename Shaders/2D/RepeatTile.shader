//only for 2D stuff
Shader "M8/2D/RepeatTile" 
{
	Properties 
	{
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		modColor ("Mod Color", Color) = (1.0, 1.0, 1.0, 1.0)
		tileX ("Tile X", Float) = 1
		tileY ("Tile Y", Float) = 1
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
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
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct Output {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			
			float4 modColor;

			float tileX;
			float tileY;
			
			Output vtx(Input IN)
			{
				Output o;
				
				o.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				o.color = IN.color;
				
				o.texcoord = IN.texcoord;
				o.texcoord.x *= tileX;
				o.texcoord.y *= tileY;
				
				return o;
			}
			
			fixed4 frag(Output i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color * modColor;
				return col;
			}
			
			ENDCG
		}
	}
	
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Blend One OneMinusSrcAlpha Cull Off Fog { Mode Off } 
		LOD 100
		
		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
			Bind "Color", color
		}

		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] { combine texture * primary } 
		}
	}
}
