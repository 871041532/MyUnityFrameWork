
Shader "MathLearn/平移测试"
{
    Properties
    {
        m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
        m_Line1("Line1 Axis", Color) = (1, 0, 0, 0)
        m_Line2("Line2 Axis", Color) = (0, 1, 0, 0)
        m_Line3("Line3 Axis", Color) = (0, 0, 1, 0)
        m_Line4("Line4 Axis", Color) = (0, 0, 0, 1)
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
            fixed4 m_Line1;
            fixed4 m_Line2;
            fixed4 m_Line3;
            fixed4 m_Line4;

            struct v2f
            {
                float4 pos:SV_POSITION;
                fixed3 color:COLOR;
            };

            v2f vert(appdata_base v)
            {
                 v2f o;
                 float4x4 transMatrix = float4x4(m_Line1, m_Line2, m_Line3, m_Line4);
                 float3 posTemp = mul(float4(v.vertex.xyz, 1), transMatrix).xyz;
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
