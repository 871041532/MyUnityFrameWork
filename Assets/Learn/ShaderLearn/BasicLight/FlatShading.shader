// 逐顶点高光
Shader "ShaderLearn/FlatShading"
{
    Properties
    {
        m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
        m_Specular("Specular Color", Color) = (1, 1, 1, 1)
        m_Gloss("Gloss", Range(8, 256)) = 20
    }
    SubShader
    {
        Pass{
        	//设置光照模式，定义了正确的LightMode才能得到unity内置光照变量
            Tags{"LightMode"="ForwardBase"}
            CGPROGRAM
            #include "UnityCg.cginc"
            // 导入此文件才能使用内置光照
            #include "Lighting.cginc"  
            #pragma  vertex vert
            #pragma fragment frag         
           
            fixed4 m_DiffuseColor;
            fixed4 m_Specular;
            float m_Gloss; 
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float3 worldPos:TEXCOORD1;
            };                                       
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                float3 worldDx = ddx(i.worldPos);
                float3 worldDy = ddy(i.worldPos);
                float3 worldNormal = normalize(cross(worldDy, worldDx));
//                return fixed4(worldNormal * 0.5 + 0.5,0.0);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                // 漫反射
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * m_DiffuseColor.rgb * saturate(dot(worldNormal, worldLight)); 
                // 高光
                fixed3 reflectDir = normalize(reflect(-worldLight, worldNormal));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                fixed3 specular = _LightColor0.rgb * m_Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), m_Gloss);
                
                return fixed4((ambient + diffuse + specular), 1.0);
            }
                                 
            ENDCG
        }
    }
   FallBack Off
}
