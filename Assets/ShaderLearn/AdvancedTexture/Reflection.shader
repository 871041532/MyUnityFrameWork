//  逐像素漫反射 + 环境映射反射 + 阴影 
Shader "ShaderLearn/Reflection"
{
    Properties 
    {
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _ReflectColor("Reflection Color", Color) = (1, 1, 1, 1)  // 反射颜色
        _ReflectAmount("Reflect Amount", Range(0, 1)) = 1  // 反射程度
        _CubeMap("Reflection CubeMap", Cube) = "_Skybox" {}  // 环境映射纹理               
    }
    
    SubShader 
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry"}
        Pass 
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            fixed3 _Color;
            fixed3 _ReflectColor;
            fixed  _ReflectAmount;
            samplerCUBE _CubeMap;
            
            struct v2f
            {
                float4 pos:SV_POSITION;
                float3 worldPos:TEXCOORD0;
                fixed3 worldNormal:TEXCOORD1;
                fixed3 worldViewDir:TEXCOORD2;
                fixed3 worldRefl:TEXCOORD3;
                SHADOW_COORDS(4)
            };
            
            v2f vert(appdata_base v)
            {
               v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
               o.worldNormal = UnityObjectToWorldNormal(v.normal);
               o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
               o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
               // 使用reflect函数计算反射方向
               o.worldRefl = reflect(-o.worldViewDir, o.worldNormal);
               TRANSFER_SHADOW(o);  
               return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(i.worldViewDir);
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * saturate(dot(worldNormal, worldLightDir));
                
                // 对立方体纹理进行采样获取反射
                fixed3 reflection = texCUBE(_CubeMap, i.worldRefl).rgb * _ReflectColor.rgb;
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                // 使用_ReflectAmount混合漫反射颜色和反射颜色，并和环境光相加后返回
                fixed3 color = ambient + lerp(diffuse, reflection, _ReflectAmount) * atten;
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Reflective/VertexLit"
}
