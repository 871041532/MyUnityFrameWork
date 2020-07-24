Shader "ShaderLearn/ToonShading"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex (RGB)", 2D) = "white" {}
        _Ramp("Ramp Texture", 2D) = "white"{}  // 漫反射渐变纹理
        _Outline("Outline", Range(0, 1)) = 0.1  // 轮廓线宽度
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)  // 轮廓线颜色
        _Specular("Specular", Color) = (1, 1, 1, 1)  // 高光反射颜色
        _SpecularScale("Specular Scale", Range(0, 0.1)) = 0.01  // 高光反射阈值
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        CGINCLUDE
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc" 
        fixed4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        sampler2D _Ramp;
        float4 _Ramp_ST;
        float _Outline;
        fixed4 _OutlineColor;
        fixed4 _Specular;
        fixed _SpecularScale; 
        ENDCG
        
        Pass
        {
            NAME "OUTLINE"  // 描边在非真实感渲染中是非常常见的效果，定义名称可以在后面的使用中不需要再重复编写此Pass，而只需要调用它的名字即可
            Cull Front  // 剔除正面，只渲染背面
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f 
            {
                float4 pos:SV_POSITION;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                // 首先把顶点和法线变换到视角空间下，这是为了让描边可以在观察空间达到最好的效果。
                float3 pos = UnityObjectToViewPos(v.vertex); 
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                // 法线的z分量，对其归一化后再将顶点沿其方向扩张，得到扩张后的顶点坐标, 对法线的处理是为了尽可能避免背面扩张后的顶点挡住正面的面片
                normal.z = -0.5;
                pos = pos + normalize(normal) * _Outline;
                // 最后，把顶点从视角空间变换到裁剪空间。
                o.pos = mul(UNITY_MATRIX_P, float4(pos, 1.0));
                return o;
            }
            
            float4 frag(v2f i):SV_Target
            {
                return float4(_OutlineColor.rgb, 1);
            }
            ENDCG   
        }
        
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
                SHADOW_COORDS(3)
            };
            
            // 计算了世界空间下的法线方向和顶点位置，
            // 使用Unity提供的内置宏SHADOW_COORDS和TRANSFER_SHADOW来计算阴影所需的各个变量
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }
            
            float4 frag(v2f i):SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);
                
                fixed4 c = tex2D(_MainTex, i.uv);
                fixed3 albedo = c.rgb * _Color.rgb;
                // 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
               
                // 漫反射
                fixed diff =  dot(worldNormal, worldLightDir);    
                diff = (diff * 0.5 + 0.5) * atten;
                fixed diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, float2(diff, diff)).rgb; 
                // 高光, 使用fwidth对高光区域的边界进行抗锯齿处理
                fixed spec = dot(worldNormal, worldHalfDir);
                fixed w = fwidth(spec) * 2.0;
                // 使用了step(0.000 1, _SpecularScale)，这是为了在_SpecularScale为0时，可以完全消除高光反射的光照。
                fixed3 specular = _Specular.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"  // 这对产生正确的阴影投射效果很重要
}