// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:Legacy Shaders/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33866,y:32884,varname:node_9361,prsc:2|emission-9614-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:8068,x:32475,y:33105,varname:node_8068,prsc:2;n:type:ShaderForge.SFN_LightColor,id:3406,x:32475,y:32971,varname:node_3406,prsc:2;n:type:ShaderForge.SFN_LightVector,id:6869,x:32263,y:32778,varname:node_6869,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9684,x:32263,y:32906,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:7782,x:32475,y:32821,cmnt:Lambert,varname:node_7782,prsc:2,dt:1|A-6869-OUT,B-9684-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:33161,y:32369,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:5927,x:33161,y:32554,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:5085,x:32876,y:32921,cmnt:Attenuate and Color,varname:node_5085,prsc:2|A-7782-OUT,B-3406-RGB,C-8068-OUT;n:type:ShaderForge.SFN_AmbientLight,id:7528,x:33161,y:32701,varname:node_7528,prsc:2;n:type:ShaderForge.SFN_Multiply,id:544,x:33361,y:32554,cmnt:Surface,varname:node_544,prsc:2|A-851-RGB,B-5927-RGB;n:type:ShaderForge.SFN_Tex2d,id:9722,x:33173,y:33548,ptovrint:False,ptlb:Hatch,ptin:_Hatch,varname:node_9722,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0bd5ffeb37918fb429cbbd9f0fd8bc13,ntxv:0,isnm:False|UVIN-8735-UVOUT;n:type:ShaderForge.SFN_Code,id:926,x:32925,y:33145,varname:node_926,prsc:2,code:IwBpAGYAIABkAGUAZgBpAG4AZQBkACgAVQBOAEkAVABZAF8ATgBPAF8ATABJAE4ARQBBAFIAXwBDAE8ATABPAFIAUwBQAEEAQwBFACkACgAgACAAIAAgAHIAZQB0AHUAcgBuACAAZABvAHQAKABjACwAIAB1AG4AaQB0AHkAXwBDAG8AbABvAHIAUwBwAGEAYwBlAEwAdQBtAGkAbgBhAG4AYwBlAC4AcgBnAGIAKQA7AAoAIwBlAGwAcwBlAAoAIAAgACAAIABjACAAKgA9ACAAdQBuAGkAdAB5AF8AQwBvAGwAbwByAFMAcABhAGMAZQBMAHUAbQBpAG4AYQBuAGMAZQAuAHIAZwBiADsACgAgACAAIAAgAGgAYQBsAGYAIAByAGUAcQB1AGkAcgBlAHMATABpAG4AZQBhAHIARgBpAHgAdQBwACAAPQAgAHUAbgBpAHQAeQBfAEMAbwBsAG8AcgBTAHAAYQBjAGUATAB1AG0AaQBuAGEAbgBjAGUALgB3ADsACgAgACAAIAAgAHIAZQB0AHUAcgBuACAAYwAuAHgAIAArACAAYwAuAHkAIAArACAAYwAuAHoAIAArACAAMgAgACoAIABzAHEAcgB0ACgAYwAuAHkAIAAqACAAKABjAC4AeAAgACsAIABjAC4AegApACkAIAAqACAAcgBlAHEAdQBpAHIAZQBzAEwAaQBuAGUAYQByAEYAaQB4AHUAcAA7AAoAIwBlAG4AZABpAGYA,output:4,fname:Luminance,width:247,height:112,input:2,input_1_label:c|A-5085-OUT;n:type:ShaderForge.SFN_Code,id:1675,x:32901,y:33316,varname:node_1675,prsc:2,code:aABhAGwAZgA0ACAAcwBoAGEAZABpAG4AZwBGAGEAYwB0AG8AcgAgAD0AIABoAGEAbABmADQAKABzAGgAYQBkAGkAbgBnAC4AeAB4AHgAeAApADsACgBjAG8AbgBzAHQAIABoAGEAbABmADQAIABsAGUAZgB0AFIAbwBvAHQAIAA9ACAAaABhAGwAZgA0ACgALQAwAC4AMgA1ACwAIAAwAC4AMAAsACAAMAAuADIANQAsACAAMAAuADUAKQA7AAoAYwBvAG4AcwB0ACAAaABhAGwAZgA0ACAAcgBpAGcAaAB0AFIAbwBvAHQAIAA9ACAAaABhAGwAZgA0ACgAMAAuADIANQAsACAAMAAuADUALAAgADAALgA3ADUALAAgADEALgAwACkAOwAKAAoAcgBlAHQAdQByAG4AIAA0AC4AMAAgACoAIABjAGwAYQBtAHAAKABzAGgAYQBkAGkAbgBnAEYAYQBjAHQAbwByACAALQAgAGwAZQBmAHQAUgBvAG8AdAAsACAAMAAsACAAcgBpAGcAaAB0AFIAbwBvAHQAIAAtACAAcwBoAGEAZABpAG4AZwBGAGEAYwB0AG8AcgApADsA,output:7,fname:ShadeWeights,width:405,height:133,input:4,input_1_label:shading|A-926-OUT;n:type:ShaderForge.SFN_Code,id:9614,x:33400,y:33340,varname:node_9614,prsc:2,code:CgBoAGEAbABmADQAIABoAGEAdABjAGgAIAA9ACAAaABhAGwAZgA0ACgAaABhAHQAYwBoAEEALAAgAGgAYQB0AGMAaABSAEcAQgAuAGIAZwByACkAOwAKAGgAYQBsAGYAIAB0ACAAPQAgAHMAYQB0AHUAcgBhAHQAZQAoAHcAZQBpAGcAaAB0AHMALgB4ACsAdwBlAGkAZwBoAHQAcwAuAHkAKwB3AGUAaQBnAGgAdABzAC4AegArAHcAZQBpAGcAaAB0AHMALgB3ACkAOwAKAGgAYQBsAGYAIABzAGgAYQBkAGUAIAA9ACAAbABlAHIAcAAoADEALAAgAGQAbwB0ACgAdwBlAGkAZwBoAHQAcwAsACAAaABhAHQAYwBoACkALAAgAHQAKQA7AAoAcgBlAHQAdQByAG4AIABoAGEAbABmADMAKABzAGgAYQBkAGUALgB4AHgAeAApADsA,output:6,fname:HatchWeight,width:344,height:161,input:7,input:6,input:4,input_1_label:weights,input_2_label:hatchRGB,input_3_label:hatchA|A-1675-OUT,B-9722-RGB,C-9722-A;n:type:ShaderForge.SFN_TexCoord,id:8735,x:32922,y:33548,varname:node_8735,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:2461,x:32650,y:33313,varname:node_2461,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:8810,x:33626,y:33025,varname:node_8810,prsc:2|A-544-OUT,B-9614-OUT;proporder:851-5927-9722;pass:END;sub:END;*/

Shader "M8/Toon/CrossHatch" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Hatch ("Hatch", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Hatch; uniform float4 _Hatch_ST;
            half Luminance( float3 c ){
            #if defined(UNITY_NO_LINEAR_COLORSPACE)
                return dot(c, unity_ColorSpaceLuminance.rgb);
            #else
                c *= unity_ColorSpaceLuminance.rgb;
                half requiresLinearFixup = unity_ColorSpaceLuminance.w;
                return c.x + c.y + c.z + 2 * sqrt(c.y * (c.x + c.z)) * requiresLinearFixup;
            #endif
            }
            
            half4 ShadeWeights( half shading ){
            half4 shadingFactor = half4(shading.xxxx);
            const half4 leftRoot = half4(-0.25, 0.0, 0.25, 0.5);
            const half4 rightRoot = half4(0.25, 0.5, 0.75, 1.0);
            
            return 4.0 * clamp(shadingFactor - leftRoot, 0, rightRoot - shadingFactor);
            }
            
            half3 HatchWeight( half4 weights , half3 hatchRGB , half hatchA ){
            
            half4 hatch = half4(hatchA, hatchRGB.bgr);
            half t = saturate(weights.x+weights.y+weights.z+weights.w);
            half shade = lerp(1, dot(weights, hatch), t);
            return half3(shade.xxx);
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _Hatch_var = tex2D(_Hatch,TRANSFORM_TEX(i.uv0, _Hatch));
                float3 node_9614 = HatchWeight( ShadeWeights( Luminance( (max(0,dot(lightDirection,normalDirection))*_LightColor0.rgb*attenuation) ) ) , _Hatch_var.rgb , _Hatch_var.a );
                float3 emissive = node_9614;
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Legacy Shaders/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
