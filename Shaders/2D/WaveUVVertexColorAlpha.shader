// unlit, vertex colour, premultiplied alpha blend
// 2D stuff
Shader "M8/2D/WaveUVVertexColorAlpha" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		
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
		
		ZWrite Off 
		Lighting Off 
		Cull Off 
		Blend One OneMinusSrcAlpha
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			float speedX;
			float speedY;
			
			float amplitudeX;
			float amplitudeY;
		
			float rangeX;
			float rangeY;

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

			v2f_vct vert(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				
				float2 texCoord = float2(
					v.texcoord.x + sin(v.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
					v.texcoord.y + sin(v.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
					
				o.texcoord = TRANSFORM_TEX(texCoord, _MainTex);
				
				return o;
			}

			fixed4 frag(v2f_vct i) : COLOR
			{
				fixed4 col = i.color * tex2D(_MainTex, i.texcoord);
				col.rgb *= col.a;
				return col;
			}
			
			ENDCG
		} 
	}
}
