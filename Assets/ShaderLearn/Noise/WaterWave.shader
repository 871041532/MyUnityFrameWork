// 水波效果。环境纹理做菲涅尔反射，GrabPass做折射
Shader "ShaderLearn/WaterWave"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)  // _Color控制水面颜色
        _MainTex ("Base (RGB)", 2D) = "white" {}  // 水面波纹材质纹理
        _WaveMap("Wave Map", 2D) = "bump" {}  // 噪声纹理生成的法线纹理
        _Cubemap("Environment _Cubemap", Cube) = "_Skybox" {}  // 模拟反射的立方体纹理
        _WaveXSpeed("Wave Horizontal Speed", Range(-0.1, 0.1)) = 0.01  // 法线纹理在X上的平移速度
        _WaveYSpeed("Wave Vertical Speed", Range(-0.1, 0.1)) = 0.01  // 法线纹理在Y方向上的平移速度
        _Distortion("Distortion", Range(0, 100)) = 10  // 折射时图像的扭曲程度
    }
    SubShader
    {
         // We must be transparent, so other objects are drawn before this one.
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        // This pass grabs the screen behind the object into a texture.    
        // We can access the result in the next pass as _RefractionTex
        GrabPass { "_RefractionTex" }
        Pass 
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _WaveMap;
            float4 _WaveMap_ST;
            samplerCUBE _Cubemap;
            fixed _WaveXSpeed;
            fixed _WaveYSpeed;
            float _Distortion;
            sampler2D _RefractionTex;
            float4 _RefractionTex_TexelSize;  // 纹素大小，对屏幕图像采样坐标进行偏移时使用该变量
            
            struct v2f
            {
                float4 pos:SV_POSITION;
                float4 scrPos:TEXCOORD0;
                float4 uv:TEXCOORD1;
                float4 TtoW0:TEXCOORD2;
                float4 TtoW1:TEXCOORD3;
                float4 TtoW2:TEXCOORD4;
            };
            
            v2f vert(appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeGrabScreenPos(o.pos);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _WaveMap);
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
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                float2 speed = _Time.y * float2(_WaveXSpeed, _WaveYSpeed);
                // 获取世界空间下的法线
                fixed3 bump1 = UnpackNormal(tex2D(_WaveMap, i.uv.zw + speed)).rgb;
                fixed3 bump2 = UnpackNormal(tex2D(_WaveMap, i.uv.zw - speed)).rgb;
                fixed3 bump = normalize(bump1 + bump2);
                // 计算切线空间下的偏移，从而计算折射
                float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
                i.scrPos.xy = offset * i.scrPos.z + i.scrPos.xy;
                fixed3 refrCol = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).rgb;
                // 在世界空间下cube采样计算反射
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
                fixed4 texColor = tex2D(_MainTex, i.uv.xy + speed);
                fixed3 refDir = reflect(-viewDir, bump);
                fixed3 refCol = texCUBE(_Cubemap, refDir).rgb * texColor.rgb * _Color.rgb;
                fixed3 fresnel = pow(1- saturate(dot(viewDir, bump)), 4);
                fixed3 finalColor = refCol * fresnel + refrCol * (1 - fresnel);
                return fixed4(finalColor, 1);
            }
            
            ENDCG        
        }
    }
    FallBack "Diffuse"
}
