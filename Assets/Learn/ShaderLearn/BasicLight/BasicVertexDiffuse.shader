﻿// 基础光照，逐顶点漫反射
Shader "ShaderLearn/BasicVertexDiffuse"
{
    Properties
    {
        m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
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
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                fixed3 color:COLOR;
            };
            
            v2f vert(appdata_base v)
            {
                 v2f o;
                 o.pos =  UnityObjectToClipPos(v.vertex);
                 fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                 // 获得世界坐标系下的法线
                 fixed3 wordNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                // 光源方向
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz); 
                // 获得漫反射光
                fixed3 diffuse = _LightColor0.rgb * m_DiffuseColor.rgb * saturate(dot(wordNormal, worldLight));
                o.color = ambient + diffuse;
                 return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                return fixed4(i.color, 1.0);   
            }
                                 
            ENDCG
        }
    }
//    FallBack "Diffuse"
}
