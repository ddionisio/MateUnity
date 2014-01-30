Shader "M8/ProBuilder/Diffuse Vertex Color Animate" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Width("Texture Width", Float) = 0
    _FPS("Frame Per Second", Float) = 6
    _Count("Frame Count", Float) = 4
  }
  SubShader {
    Tags { "RenderType" = "Opaque" }

    ColorMask RGB

    CGPROGRAM
    #pragma surface surf Lambert vertex:vtx addshadow fullforwardshadows

    sampler2D _MainTex;
    float _Width;
    float _FPS;
    float _Count;

    struct Input {
        float4 color : COLOR; // interpolated vertex color
        float2 uv_MainTex;
    };
    
    void vtx(inout appdata_full v) {
    	
		v.texcoord.x = (v.texcoord.x + round(_FPS * _Time.y))/_Count;
	}

    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
        o.Albedo = c.rgb;
    }
    ENDCG
  }
  Fallback "Diffuse"
}
