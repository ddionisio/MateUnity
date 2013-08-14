//only for 2D stuff
Shader "M8/LitRepeatTile3D" 
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
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vtx
		
		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR;
		};
		
		sampler2D _MainTex;
		
		fixed4 modColor;

		float tileX;
		float tileY;
		
		void vtx(inout appdata_full v) {
			v.texcoord.x *= tileX;
			v.texcoord.y *= tileY;
		}
		
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color * modColor;
			o.Albedo = mainColor.rgb;
			o.Alpha = mainColor.a;
		}
		
		ENDCG
	}
	
	Fallback "RepeatTile3D"
}
