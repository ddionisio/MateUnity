// unlit, vertex colour, premultiplied alpha blend

Shader "M8/2D/PremulVertexColorTile" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend One OneMinusSrcAlpha
		LOD 110
				
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				
				return col;
			}
		
			ENDCG
		}
		
		GrabPass { }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
						
			sampler2D _GrabTexture : register(s0);
			float tile;
			
			struct vin_vct 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				float4 screenPos : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				if(tile > 0.0) {
					half size = 1.0 / tile;
					half2 pBase = i.screenPos.xy - fmod(i.screenPos.xy, size.xx);
					half2 pCenter = pBase + (size/2.0).xx;
					return tex2D(_GrabTexture, pCenter);
				}
				else {
					return tex2D(_GrabTexture, i.screenPos.xy);
				}
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
