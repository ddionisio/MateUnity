Shader "M8/Sprite/ZTint"
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

		_ZOfs("Z Offset", Float) = 0
		_ZColor("Tint Z", Color) = (0.5,0.5,0.5,0.3)
		_ZUnit("Z Measurement", Float) = 0.5
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
			#pragma vertex SpriteVert_zTint
			#pragma fragment SpriteFrag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _ZColor;
			float _ZOfs;			 //this is the focused world z
			float _ZUnit;

			v2f SpriteVert_zTint(appdata_t IN)
			{
				v2f OUT = SpriteVert(IN);

				//modify color based on z axis
				float wpz = dot(unity_ObjectToWorld._m20_m21_m22, IN.vertex) + unity_ObjectToWorld._m23;

				OUT.color *= lerp(fixed4(1, 1, 1, 1), _ZColor, clamp(abs(wpz - _ZOfs) / _ZUnit, 0, 1));

				return OUT;
			}
		ENDCG
		}
	}
}
