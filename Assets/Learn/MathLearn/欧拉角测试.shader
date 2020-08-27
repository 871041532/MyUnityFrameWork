
Shader "MathLearn/欧拉角测试"
{
    Properties
    {
        m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
        m_Heading("Heading", float) = 0
        m_Pitch("Pitch", float) = 0
        m_Bank("Bank", float) = 0
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
            float m_Heading;
            float m_Pitch;
            float m_Bank;

            struct v2f
            {
                float4 pos:SV_POSITION;
                fixed3 color:COLOR;
            };

            v2f vert(appdata_base v)
            {
                 v2f o;
                 float radHeading = radians(m_Heading);
                 float radPitch = radians(m_Pitch);
                 float radBank = radians(m_Bank);
                 
                 float cosHeading = cos(radHeading);
                 float sinHeading = sin(radHeading);
                 
                 float cosPitch = cos(radPitch);
                 float sinPitch = sin(radPitch);
                 
                 float cosBank = cos(radBank);
                 float sinBank = sin(radBank);
                 
                 float3x3 rotationHeading = float3x3(float3(cosHeading, 0, -sinHeading), float3(0, 1, 0), float3(sinHeading, 0, cosHeading));
                 float3x3 rotationPitch = float3x3(float3(1, 0, 0), float3(0, cosPitch, sinPitch), float3(0, -sinPitch, cosPitch));
                 float3x3 rotationBank = float3x3(float3(cosBank, sinBank, 0), float3(-sinBank, cosBank, 0), float3(0, 0, 1));
                 
                 float3x3 compositeMatrix = mul(rotationBank, rotationPitch);
                 compositeMatrix = mul(compositeMatrix, rotationHeading);
                 float3 posTemp = mul(v.vertex, compositeMatrix);   // * rotationPitch * rotationHeading       
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
