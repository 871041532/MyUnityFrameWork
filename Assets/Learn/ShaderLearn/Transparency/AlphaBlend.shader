// 深度混合
Shader "ShaderLearn/AlphaBlend"
{
    Properties
    {
        m_Color("Color Tint", Color) = (1, 1, 1, 1)
        m_MainTex("Main Texture", 2D) = "white"{}
        m_AlphaScale("Alpha Scale", Range(0, 1)) = 1  // 控制整体的透明度
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            ZWrite Off  // 关闭深度写入
            Blend SrcAlpha OneMinusSrcAlpha  // 设置透明混合模式
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 m_Color;
            sampler2D m_MainTex;
            float4 m_MainTex_ST;
            fixed m_AlphaScale;
            
            struct v2f{
                float4 pos:SV_POSITION;
                float3 worldPos:TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
                float2 uv:TEXCOORD2;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.texcoord.xy * m_MainTex_ST.xy + m_MainTex_ST.zw;
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 texColor = tex2D(m_MainTex, i.uv);
                fixed3 albedo = texColor.rgb * m_Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(worldNormal, worldLightDir));
                return fixed4(ambient + diffuse, texColor.a * m_AlphaScale);
            }
            
            ENDCG  
        }
    }
    
    Fallback Off
}