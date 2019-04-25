//Only use for sprites
Shader "M8/Sprite/ColorOverlayPulseAdd"
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

		_ColorOverlayMin("Overlay Min", Color) = (0,0,0)
		_ColorOverlayMax("Overlay Max", Color) = (0,0,0)
		_PulseScale("Pulse Scale", Float) = 1
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
				#pragma vertex SpriteVert_OverlayColorAdd
				#pragma fragment SpriteFrag_OverlayColorAdd
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_local _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				fixed3 _ColorOverlayMin;
				fixed3 _ColorOverlayMax;
				float _PulseScale;

				struct v2fExt
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR0;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_OUTPUT_STEREO

					fixed3 colorOverlay : COLOR1;
				};

				v2fExt SpriteVert_OverlayColorAdd(appdata_t IN)
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

					float sinT = sin(_Time.y * _PulseScale);
					float t = sinT * sinT;

					OUT.colorOverlay = lerp(_ColorOverlayMin, _ColorOverlayMax, t);

					return OUT;
				}

				fixed4 SpriteFrag_OverlayColorAdd(v2fExt IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb = clamp(c.rgb + IN.colorOverlay, 0, 1);
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}
		}
}