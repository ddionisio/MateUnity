//only for 2D stuff
Shader "M8/2D/WaveWorld" 
{
	Properties 
	{
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		modColor ("Mod Color", Color) = (1.0, 1.0, 1.0, 1.0)
		
		speedX ("Speed X", Float) = 1
		speedY ("Speed Y", Float) = 1
		
		amplitudeX ("amplitude X", Float) = 0.017453292
		amplitudeY ("amplitude Y", Float) = 0.017453292
	
		rangeX ("range X", Float) = 0.017453292
		rangeY ("range Y", Float) = 0.017453292
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
			};
			
			struct Output {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 modColor;
			
			float speedX;
			float speedY;
			
			float amplitudeX;
			float amplitudeY;
		
			float rangeX;
			float rangeY;
			
			Output vtx(Input IN)
			{
				Output o;
				
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.color = IN.color;
				
				float4 wVtx = mul(unity_ObjectToWorld, float4(IN.vertex.xyz, 1.0));
				
				o.texcoord = TRANSFORM_TEX(wVtx.xy, _MainTex);
				
				return o;
			}
			
			fixed4 frag(Output i) : COLOR
			{
				float2 texCoord = float2(
					i.texcoord.x + sin(i.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
					i.texcoord.y + sin(i.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
				
				fixed4 col = tex2D(_MainTex, texCoord) * i.color * modColor;
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
