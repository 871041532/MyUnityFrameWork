// 基于Blinn-Phong 世界空间下的法线纹理
Shader "ShaderLearn/NormalMapInWorldSpace"{
    Properties{
        m_Color("Color Ting", Color) = (1, 1, 1, 1)
        m_MainTex("Main Tex", 2D) = "white"{}
        m_Specular("Specular", Color) = (1, 1, 1, 1)
        m_Gloss("Gloss", Range(8.0, 256)) = 20
        m_BumpMap("Normal Map", 2D) = "bump" {}
        m_BumpScale("Bump Scale", float) = 1.0
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
            
            struct v2f  
            {
                float4 pos: SV_POSITION;
                float4 uv: TEXCOORD0;
                float4 t2w0: TEXCOORD1;  // 矩阵第一行
                float4 t2w1: TEXCOORD2;  // 矩阵第二行
                float4 t2w2:TEXCOORD3;  // 矩阵第三行
            };
            
            v2f vert(appdata_tan v) 
            {
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.texcoord.xy * m_MainTex_ST.xy + m_MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * m_BumpMap_ST.xy + m_BumpMap_ST.zw;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                // 切线转世界，知道切线在世界中表示，转到列。w分量用于存储世界空间下的顶点位置。
                o.t2w0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x); 
                o.t2w1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.t2w2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);   
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                float3 worldPos = float3(i.t2w0.w, i.t2w1.w, i.t2w2.w);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                
                fixed3 bump = UnpackNormal(tex2D(m_BumpMap, i.uv.zw));
                bump.xy *= m_BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
                // 从切线空间转换到法线空间
                bump = normalize(half3(dot(i.t2w0.xyz, bump), dot(i.t2w1.xyz, bump), dot(i.t2w2.xyz, bump)));
                
                 // 漫反射
                fixed3 albedo = tex2D(m_MainTex, i.uv.xy).rgb * m_Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(bump, lightDir));
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                // 高光
                fixed3 halfDir = normalize(lightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(bump, halfDir)), m_Gloss);
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}