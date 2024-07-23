//Note: This is only useful for non-atlas sprites, or for plugins that assume the use of sprite shaders
Shader "M8/Sprite/UVWaveXY"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		
		[Header(Blending)]
    		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source", Int) = 5
    		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination", Int) = 10
		
		[Header(Wave)]
		speedX("Speed X", Float) = 1
        speedY("Speed Y", Float) = 1

        amplitudeX("Amplitude X", Float) = 0.017453292
        amplitudeY("Amplitude Y", Float) = 0.017453292

        rangeX("Range X", Float) = 0.017453292
        rangeY("Range Y", Float) = 0.017453292
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
        Blend [_BlendSrc] [_BlendDst]

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert_UVWave
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
			
			float speedX;
			float speedY;
			
			float amplitudeX;
			float amplitudeY;
		
			float rangeX;
			float rangeY;
			
			v2f SpriteVert_UVWave(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID (IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
				float2 waveTexCoord = float2(
					IN.texcoord.x + sin(IN.texcoord.y*rangeY + speedX*_Time.y)*amplitudeX,
					IN.texcoord.y + sin(IN.texcoord.x*rangeX + speedY*_Time.y)*amplitudeY);
				
				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = waveTexCoord;
				OUT.color = IN.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}
        ENDCG
        }
    }
}