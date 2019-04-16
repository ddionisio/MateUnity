Shader "M8/Sprite/Dissolve"
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

		_DissolveTex("Dissolve Overlay", 2D) = "white" {}
		_EmissionColor("Emission Color", color) = (1,0,0,1)
		_EmissionThickness("Emission Thickness", Range(0, 1)) = 0.1
		_DissolvePower("Dissolve Power", Range(0, 1)) = 1
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
			#pragma vertex SpriteVert_Dissolve
			#pragma fragment SpriteFrag_Dissolve
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			sampler2D _DissolveTex;
			float4 _DissolveTex_ST;

			fixed4 _EmissionColor;
			fixed _EmissionThickness;
			fixed _DissolvePower; //start at 1 then gradually to 0

			struct v2fExt
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2fExt SpriteVert_Dissolve(appdata_t IN)
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

				//coord for dissolve to accomodate texture scale/offset
				OUT.texcoord2 = IN.texcoord * _DissolveTex_ST.xy + _DissolveTex_ST.zw;

				return OUT;
			}

			fixed4 SpriteFrag_Dissolve(v2fExt IN) : COLOR
			{
				fixed4 clr = SampleSpriteTexture(IN.texcoord) * IN.color;
				fixed mask = tex2D(_DissolveTex, IN.texcoord2).r;

				fixed4 blend = fixed4(0,0,0,0);
				if (mask < _DissolvePower + _EmissionThickness)
					blend = fixed4(_EmissionColor.r, _EmissionColor.g, _EmissionColor.b, clr.a);
				if (mask <= _DissolvePower)
					blend = clr;

				blend.rgb *= blend.a;

				return blend;
			}
		ENDCG
		}
	}
}
