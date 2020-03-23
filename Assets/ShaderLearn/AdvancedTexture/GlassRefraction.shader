 // 玻璃效果：世界空间下计算法线折射 + 立方体纹理采样反射
Shader "ShaderLearn/GlassRefraction"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Cubemap("Refraction Cubemap", Cube) = "_Skybox" {}  // 环境映射纹理
        _Distortion("Distortion", Range(0, 100)) = 10  //控制模拟折射时图像的扭曲程度
        _RefractAmount("Refraction Amount", Range(0, 1)) = 1  //控制折射程度，0只有反射效果，1只有折射效果
    }
    
    SubShader
    {
        // We must be transparent, so other objects are drawn before this one.
        Tags {"RenderType" = "Opaque" "Queue" = "Transparent"}
        // This pass grabs the screen behind the object into a texture.
        // We can access the result in the next pass as _RefractionTex 
        GrabPass { "_RefractionTex" }
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            samplerCUBE _Cubemap;
            float _Distortion;
            fixed _RefractAmount;
            sampler2D _RefractionTex;  // 对应GrabPass指定的纹理名称
            float4 _RefractionTex_TexelSize;  // 对应纹理的纹素大小
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float4 uv:TEXCOORD0;
                float4 TtoW0:TEXCOORD1;
                float4 TtoW1:TEXCOORD2;
                float4 TtoW2:TEXCOORD3;
                float4 scrPos:TEXCOORD4;
            };
            
            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeGrabScreenPos(o.pos);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);      
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                // 切线转世界，知道切线在世界中表示，转到列。w分量用于存储世界空间下的顶点位置。
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                // 法线
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                // 切线空间偏移, 选择使用切线空间下的法线方向来进行偏移，是因为该空间下的法线可以反映顶点局部空间下的法线方向。
                float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
                i.scrPos.xy = offset + i.scrPos.xy;
                // 计算折射, 对scrPos透视除法得到真正的屏幕坐标
                fixed3 refraction = tex2D(_RefractionTex, i.scrPos.xy/i.scrPos.w).rgb;
                // 将法线从切线空间转到世界空间下
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
                // 计算反射
                fixed3 reflDir = reflect(-worldViewDir, bump);
                fixed4 texColor = tex2D(_MainTex, i.uv.xy);
                fixed3 reflect = texCUBE(_Cubemap, reflDir).rgb * texColor.rgb;
                // 最终颜色 
                fixed3 finalColor = lerp(reflect, refraction, _RefractAmount);
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Reflective/VertexLit"
}
