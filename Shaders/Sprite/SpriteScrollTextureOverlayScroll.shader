//Only use for sprites
Shader "M8/Sprite/ScrollTextureOverlayScroll"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

		_Params("Params speed=(x,y) wave amp=(z,w)", Vector) = (1,1,0,0)

		_Overlay("Overlay", 2D) = "white" {}
		_OverlayColor("Overlay Color", Color) = (1,1,1,1)
		_OverlayParams("Overlay Params speed=(x,y) wave amp=(z,w)", Vector) = (1,1,0,0)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert_Overlay
			#pragma fragment SpriteFrag_Overlay
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
		
			struct v2fExt
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _Params;

			sampler2D _Overlay;
			float4 _Overlay_ST;
			float4 _OverlayParams;
			fixed4 _OverlayColor;

			v2fExt SpriteVert_Overlay(appdata_t IN)
			{
				v2fExt OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				//scroll base texture
				OUT.texcoord += _Params.xy * _Time.y + half2(_Params.z * _CosTime.w, _Params.w * _SinTime.w);

				//coord for overlay to accomodate texture scale/offset and scroll
				OUT.texcoord2 = TRANSFORM_TEX(IN.texcoord, _Overlay) + _OverlayParams.xy * _Time.y + half2(_OverlayParams.z * _CosTime.w, _OverlayParams.w * _SinTime.w);

				return OUT;
			}

			fixed4 SpriteFrag_Overlay(v2fExt IN) : COLOR
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				fixed4 o = tex2D(_Overlay, IN.texcoord2);

				c.rgb = lerp(c.rgb, o.rgb * _OverlayColor.rgb, _OverlayColor.a * o.a);

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
