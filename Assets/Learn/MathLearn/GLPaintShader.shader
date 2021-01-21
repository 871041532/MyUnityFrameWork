Shader "MathLearn/GLPaintShader"
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
             
             struct a2v {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
                float4 texcoord: TEXCOORD0;
             };
             
             struct v2f
             {
                float4 pos:SV_POSITION;
                float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
                float2 uv:TEXCOORD2;
             };
             
             v2f vert(a2v v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.worldNormal = UnityObjectToWorldNormal(v.normal);
                 o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                 // 计算最终uv坐标，先缩放再偏移，可以使用宏TRANSFORM_TEX(v.texcoord, m_MainTex)
                 o.uv = v.texcoord.xy * m_MainTex_ST.xy + m_MainTex_ST.zw; 
                 return o;
             }
             
             fixed4 frag(v2f i): SV_Target
             {
                 // fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                  fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                  // 漫反射
                  // 对纹理采样，替代漫反射颜色
                  fixed3 albedo = tex2D(m_MainTex, i.uv).rgb * m_Color.rgb;
                  fixed3 diffuse = _LightColor0.rgb * albedo; //* max(0, dot(i.worldNormal, worldLight));
                  // 环境光也受abledo影响
                  fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                  //高光
                  fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                  fixed3 halfDir = normalize(worldLight + viewDir);
                  fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(i.worldNormal, halfDir)), m_Gloss);
                  fixed4 color = fixed4((ambient + diffuse + specular), 1.0);
//                  color = m_Color;
                  return color;
             }
             ENDCG      
        }
    }
    
    Fallback Off
}
 
