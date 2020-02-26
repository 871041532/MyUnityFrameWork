 // 默认billin-Phong，简单纹理
Shader "ShaderLearn/SingleSimple"
{
    Properties {
           m_Color("Color Tint", Color) = (1, 1, 1, 1)
           m_MainTex("Main Tex", 2D) = "white" {} 
           m_Specular("Specular", Color) = (1, 1, 1, 1)
           m_Gloss("Gloss", Range(8.0, 256)) = 20
    }
    
    SubShader{
        Tags{"LightMode"="ForwardBase"}
        Pass {
             CGPROGRAM
             #include "UnityCG.cginc"
             #include "Lighting.cginc"
             #pragma vertex vert
             #pragma fragment frag
             
             float m_Gloss;
             fixed4 m_Specular;
             fixed4 m_Color;
             float4 m_MainTex_ST;
             sampler2D m_MainTex;
             
             struct v2f
             {
                float4 pos:SV_POSITION;
                float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
             };
             
             v2f vert(appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.worldNormal = UnityObjectToWorldNormal(v.normal);
                 o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                 return o;
             }
             
             fixed4 frag(v2f i):SV_Target
             {
                  fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                  fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                  // 漫反射
                  //高光
                  fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                  fixed3 halfDir = normalize(worldLight + viewDir);
                  fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(i.worldNormal, halfDir)), m_Gloss);
                  return fixed4((ambient + specular), 1.0);
             }
             ENDCG
            
        }
    }
    
    Fallback Off
}