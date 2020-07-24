// 切线空间下的法线纹理，基于bllin-Phong
Shader "ShaderLearn/NormalMapInTangentSpace"
{
    Properties {
        m_Color("Color Tint", Color) = (1, 1, 1,1)
        m_MainTex("Main Tex", 2D) = "white"{}
        m_Specular("Specular", Color) = (1, 1, 1, 1)
        m_Gloss("Gloss", Range(8.0, 256)) = 20
        m_BumpMap("Normal Map", 2D) = "bump" {}  // 使用bump作为默认值，bump是Unity内置的法线纹理，当没有提供任何法线纹理时，bump对应模型自带的法线信息
        m_BumpScale("Bump Scale", Float) = 1.0  // 控制凹凸程度，为0时该法线纹理不会对光照产生任何影响    
    }
    
    SubShader{
        Tags{"LightMode"="ForwardBase"}
        
        Pass {
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
              
             fixed4 m_Color;
             sampler2D m_MainTex;
             float4  m_MainTex_ST;
             fixed4 m_Specular;
             float m_Gloss;
             sampler2D m_BumpMap;
             float4 m_BumpMap_ST;  
             float m_BumpScale;
            
             struct v2f {
                float4 pos: SV_POSITION;
                float4 uv: TEXCOORD0;
                float3 lightDir:TEXCOORD1;
                float3 viewDir: TEXCOORD2;
             };
             
             v2f vert(appdata_tan v)  
             {
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex);
                // xy分量存储贴图纹理坐标
                o.uv.xy = v.texcoord.xy * m_MainTex_ST.xy + m_MainTex_ST.zw;
                // zw分量存储
                o.uv.zw = v.texcoord.xy * m_BumpMap_ST.xy + m_BumpMap_ST.zw;  
                // 计算副法线
                float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;  // 和切线与法线都垂直的方向有两个，w决定了选择其中哪一个
                float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal);
                // 或者直接使用宏
                // TANGENT_SPACE_ROTATION;
                // 将光照从obj转换到tangent
                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
                // 将视角方向从obj转换到tangent
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
                return o;   
             }
             
             fixed4 frag(v2f i): SV_Target
             {
                fixed3 tangentLightDir = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);
                fixed4 packedNormal = tex2D(m_BumpMap, i.uv.zw);
                fixed3 tangentNormal;
                // texture没有标记为Normal map使用下面两行
                // tangentNormal.xy = (packedNormal.xy * 2 - 1) * m_BumpScale;
                // tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));  
                // 将texture标记为Normal Map后可以使用内置函数
                tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= m_BumpScale;
                tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
                
                // 漫反射
                fixed3 albedo = tex2D(m_MainTex, i.uv.xy).rgb * m_Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(tangentNormal, tangentLightDir));
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                // 高光
                fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
                fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(tangentNormal, halfDir)), m_Gloss);
                return fixed4(ambient + diffuse + specular, 1.0);
             }
            ENDCG
        }
        
    }
    Fallback Off
}