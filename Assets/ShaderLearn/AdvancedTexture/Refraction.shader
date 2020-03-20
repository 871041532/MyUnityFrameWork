 // 逐像素漫反射 + 环境映射折射 + 阴影
Shader "ShaderLearn/Refraction"
{
    Properties
    {
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _RefractColor("Refraction Color", Color) = (1, 1, 1, 1)  // 控制反射颜色
        _RefractAmount("Refraction Amount", Range(0, 1)) = 1  // 控制材质反射程度
        _RefractRatio("Refraction Ratio", Range(0.1, 1)) = 0.5  // 不同介质的投射比  
        _Cubemap("Refraction Cubemap", Cube) = "_Skybox" {}  // 环境映射纹理
    }
    
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue" = "Geometry"}
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            fixed4 _Color;
            fixed4 _RefractColor;
            fixed _RefractAmount;
            fixed _RefractRatio;
            samplerCUBE _Cubemap;
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float3 worldPos:TEXCOORD0;
                fixed3 worldNormal:TEXCOORD1;
                fixed3 worldViewDir:TEXCOORD2;
                fixed3 worldRefr:TEXCOORD3;
                SHADOW_COORDS(4)
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
                // 使用CG的refract函数计算折射方向
                o.worldRefr = refract(-normalize(o.worldViewDir), normalize(o.worldNormal), _RefractRatio);
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
                
                // 根据折射方向对纹理进行采样，不需要归一化
                fixed3 refraction = texCUBE(_Cubemap, i.worldRefr).rgb * _RefractColor.rgb;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                // 使用_Refrac-tAmount来混合漫反射颜色和折射颜色，并和环境光照相加后返回。
                fixed3 color = ambient + lerp(diffuse, refraction, _RefractAmount) * atten;
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Reflective/VertexLit"
}
