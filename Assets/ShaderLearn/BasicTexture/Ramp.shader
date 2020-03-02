// 基于逐像素半兰伯特光照的渐变纹理Ramp
Shader "Shader Learn/Ramp"
{
    Properties{
         m_DiffuseColor("Diffuse Color", Color) = (1, 1, 1, 1)
         m_Specular("Specular", Color) = (1, 1, 1, 1)
         m_Gloss("Gloss", Range(8.0, 256)) = 20
         m_RampTex("Ramp Tex", 2D) = "white"{}
    }
    
     SubShader{
        Pass{
            Tags {"LightMode" = "ForwardBase"}
             CGPROGRAM
             #include "UnityCG.cginc"
             #include "Lighting.cginc"
             #pragma vertex vert
             #pragma fragment frag
             
             fixed4 m_DiffuseColor;
             fixed4 m_Specular;
             float m_Gloss;
             sampler2D m_RampTex;
             float4 m_RampTex_ST;
             
             struct v2f {
                float4 pos:SV_POSITION;
                float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
                // float2 uv:TEXCOORD2;
             };
             
             v2f vert(appdata_base v)
             {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // o.uv = v.texcoord.xy * m_RampTex_ST.xy + m_RampTex_ST.zw; 
                return o;
             }
             
             fixed4 frag(v2f i):SV_Target
             {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                
                // 漫反射
                fixed halfLambert = 0.5 * dot(worldNormal, worldLight) + 0.5;
                fixed3 diffuseColor = tex2D(m_RampTex, fixed2(halfLambert, halfLambert)).rgb; 
                fixed3 diffuse = _LightColor0.rgb * diffuseColor * m_DiffuseColor.rgb;
                
                // 高光
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 halfDir = normalize(worldLight + viewDir);
                fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(worldNormal, halfDir)), m_Gloss);

    		    return fixed4(ambient + diffuse + specular, 1.0);
             }
             ENDCG
        }
         
     }
   Fallback Off     
}
