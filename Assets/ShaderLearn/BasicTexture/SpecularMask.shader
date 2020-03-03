// 切线空间下的法线纹理，基于bllin-Phong, 高光遮罩
Shader "ShaderLearn/SpecularMask"
{
    Properties{
        m_Color("Color Tint", Color) = (1, 1, 1, 1)
        m_MainTex("Main Tex", 2D) = "white"{}
        m_Specular("Specular", Color) = (1, 1, 1, 1)
        m_Gloss("Gloss", Range(8.0, 256)) = 20
        m_BumpMap("Normal Map", 2D) = "bump"{}
        m_BumpScale("Bump Scale", Float) = 1.0
    }
    
    SubShader{
        Tags{"LightMode"="ForwardBase"}
        Pass{
            CGPROGRAM
            #Include "UnityCG.cginc"
            #Include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            struct v2f {
                float4 pos: SV_POSITION;
                float4 uv: TEXCOORD0;
                float3 lightDir:TEXCOORD1;
                float3 viewDir: TEXCOORD2;
            };
            
            v2f vert(appdata_tan v)
            {
            }
            
            fixed4 frag():SV_Target
            {
            }
              
            ENDCG
        }
    }
    
    Fallback Off
}
