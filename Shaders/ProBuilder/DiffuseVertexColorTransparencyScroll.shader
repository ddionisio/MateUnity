//For use with pro-builder
Shader "M8/ProBuilder/Diffuse Vertex Color Transparency Scroll" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
  }
  SubShader {
    Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

    //ZWrite Off // on might hide behind pixels, off might miss order
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB

    CGPROGRAM
    #pragma surface surf Lambert vertex:vtx

    sampler2D _MainTex;
	
	float speedX;
	float speedY;

    struct Input {
        float4 color : COLOR; // interpolated vertex color
        float2 uv_MainTex;
    };
	
	void vtx(inout appdata_full v) {
		v.texcoord.x += speedX * _Time.y;
		v.texcoord.y += speedY * _Time.y;
	}

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
    ENDCG
  }
  Fallback "Transparent/Diffuse"
}
