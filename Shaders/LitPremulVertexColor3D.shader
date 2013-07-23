Shader "M8/LitPremulVertexColor3D" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One OneMinusSrcAlpha
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	o.Albedo = mainColor.rgb;
	o.Alpha = mainColor.a;
}
ENDCG
}

Fallback "VertexLit"
}
