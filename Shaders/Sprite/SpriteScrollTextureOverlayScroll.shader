//Only use for sprites
Shader "M8/Sprite/ScrollTextureOverlayScroll"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Params("Params speed=(x,y) wave amp=(z,w)", Vector) = (1,1,0,0)
		_Overlay("Overlay", 2D) = "white" {}
		_OverlayColor("Overlay Color", Color) = (1,1,1,1)
		_OverlayParams("Overlay Params speed=(x,y) wave amp=(z,w)", Vector) = (1,1,0,0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 texcoord2  : TEXCOORD1;
			};
			
			fixed4 _Color;
			float4 _Params;

			float4 _Overlay_ST;
			float4 _OverlayParams;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				OUT.texcoord = IN.texcoord + _Params.xy*_Time.y + half2(_Params.z*_CosTime.w, _Params.w*_SinTime.w);

				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				
				OUT.texcoord2 = TRANSFORM_TEX(IN.texcoord, _Overlay) + _OverlayParams.xy*_Time.y + half2(_OverlayParams.z*_CosTime.w, _OverlayParams.w*_SinTime.w);

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _Overlay;
			fixed4 _OverlayColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed4 o = tex2D(_Overlay, IN.texcoord2);

				c.rgb = lerp(c.rgb, o.rgb * _OverlayColor.rgb, _OverlayColor.a*o.a);

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
