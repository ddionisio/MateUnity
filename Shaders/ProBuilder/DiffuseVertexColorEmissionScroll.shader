//For use with pro-builder
Shader "M8/ProBuilder/Diffuse Vertex Color Emission Scroll" {
  Properties {
    _MainTex ("Texture", 2D) = "gray" {}
	speedX ("Speed X", Float) = 1
	speedY ("Speed Y", Float) = 1
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }

    CGPROGRAM
    #pragma surface surf Lambert vertex:vtx addshadow fullforwardshadows

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
        o.Emission = c.a;
        o.Alpha = c.a; // Alpha used to control glow effect
    }
    ENDCG
  }
  Fallback "Diffuse"
}
