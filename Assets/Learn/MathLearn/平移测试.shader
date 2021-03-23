
Shader "MathLearn/平移测试"
{
    Properties
    {
        m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
        m_OffsetX("x平移", float) = 0
        m_OffsetY("y平移", float) = 0
        m_OffsetZ("z平移", float) = 0
        m_VectorW("向量w值", float) = 1
        m_MatrixW("矩阵w值", float) = 1
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
            float m_OffsetX;
            float m_OffsetY;
            float m_OffsetZ;
            float m_VectorW;
            float m_MatrixW;

            struct v2f
            {
                float4 pos:SV_POSITION;
                fixed3 color:COLOR;
            };

            v2f vert(appdata_base v)
            {
                 v2f o;
                 float4x4 offsetMatrix = float4x4(float4(1, 0, 0, 0), float4(0, 1, 0, 0), float4(0, 0, 1, 0), float4(m_OffsetX, m_OffsetY, m_OffsetZ, m_MatrixW));
                 float3 posTemp = mul(float4(v.vertex.xyz, m_VectorW), offsetMatrix).xyz + (m_VectorW / m_MatrixW - m_VectorW) * float3(m_OffsetX, m_OffsetY, m_OffsetZ);
                  o.pos =  UnityObjectToClipPos(posTemp);
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
