// 切线空间下的法线纹理，基于bllin-Phong, 高光遮罩
// 高光遮罩用来控制模型表面的高光反射强度
Shader "ShaderLearn/SpecularMask"
{
    Properties{
        m_Color("Color Tint", Color) = (1, 1, 1, 1)
        m_MainTex("Main Tex", 2D) = "white"{}
        m_Specular("Specular", Color) = (1, 1, 1, 1)
        m_Gloss("Gloss", Range(8.0, 256)) = 20
        m_BumpMap("Normal Map", 2D) = "bump"{}
        m_BumpScale("Bump Scale", Float) = 1.0
        
        m_SpecularMask("Specular Mask", 2D) = "white" {}
        m_SpecularScale("Specular Scale", Float) = 1.0
    }
    
    SubShader{
        Tags{"LightMode"="ForwardBase"}
        Pass{
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 m_Color;
            sampler2D m_MainTex;
            float4 m_MainTex_ST;
            fixed4 m_Specular;
            float m_Gloss;
            sampler2D m_BumpMap;
            float4 m_BumpMap_ST;
            float m_BumpScale;
            sampler2D m_SpecularMask;
            float4 m_SpecularMask_ST;
            float m_SpecularScale;
            
            struct v2f {
                float4 pos: SV_POSITION;
                float4 uv: TEXCOORD0;
                float3 lightDir:TEXCOORD1;
                float3 viewDir: TEXCOORD2;
                float2 uv2:TEXCOORD3;
            };
            
            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * m_MainTex_ST.xy + m_MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * m_BumpMap_ST.xy + m_BumpMap_ST.zw;
                o.uv2 = v.texcoord.xy * m_SpecularMask_ST.xy + m_SpecularMask_ST.zw;
                // 副法线
                float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w; 
                float3x3 rotationMatrix = float3x3(v.tangent.xyz, binormal, v.normal);
                o.lightDir = mul(rotationMatrix, ObjSpaceLightDir(v.vertex)).xyz;
                o.viewDir = mul(rotationMatrix, ObjSpaceViewDir(v.vertex)).xyz;
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed3 tangentLightDir = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);
                fixed4 packedNormal = tex2D(m_BumpMap, i.uv.zw);
                fixed3 tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= m_BumpScale;
                tangentNormal.z = sqrt(1.0 - dot(tangentNormal.xy, tangentNormal.xy));
                // 漫反射
                fixed3 albedo = tex2D(m_MainTex, i.uv.xy).rgb * m_Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(tangentNormal, tangentLightDir));
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                // 高光
                fixed specularMask = tex2D(m_SpecularMask, i.uv).r * m_SpecularScale;
                fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
                fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(tangentNormal, halfDir)), m_Gloss) * specularMask;
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
        }
    }
    
    Fallback Off
}
