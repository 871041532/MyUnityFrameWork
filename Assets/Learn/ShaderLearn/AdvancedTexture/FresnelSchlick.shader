 // 逐像素漫反射 + Schlick模拟菲涅尔反射 + 阴影 
Shader "ShaderLearn/FresnelSchlick"
{
    Properties
    {
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _FresnelScale("Fresnel Scale", Range(0, 1)) = 0.5
        _Cubemap("Reflection Cubemap", Cube) = "_Skybox" {}
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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            fixed4 _Color;
            fixed _FresnelScale;
            samplerCUBE _Cubemap;
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float3 worldPos:TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
                float3 worldViewDir:TEXCOORD2;
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
                // 计算反射
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
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));
                
                fixed3 reflection = texCUBE(_Cubemap, i.worldRefl).rgb;
                //使用Schlick菲涅尔近似等式来计算fresnel变量，并用它来混合漫反射光照和反射光照。
                //一些实现也会直接把fresnel和反射光照相乘叠加到漫反射光照上，模拟边缘光照的效果。
                fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1- dot(worldViewDir, worldNormal), 5);
                fixed3 color = ambient + lerp(diffuse, reflection, saturate(fresnel)) * atten;
                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Reflective/VertexLit"
}