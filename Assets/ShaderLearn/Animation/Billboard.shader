// 模型空间下的广告牌，因为在模型空间下所以不能动态合批
Shader "ShaderLearn/Billboard"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        // 用于调整是固定法线还是固定指向上的方向（约束垂直方向的程度）
        _VerticalBillboarding("Vertical Restraints", Range(0, 1)) = 1
    }
    
    SubShader
    {
        // 顶点动画需要取消批处理操作
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True"}
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _VerticalBillboarding;
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            
            v2f vert(appdata_base v)
            {
                float3 center = float3(0, 0, 0);
                float3 viewer = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                // 根据观察位置和锚点计算目标法线方向
                float3 normalDir = viewer - center;
                // 根据__VerticalBillboarding属性控制垂直方向上的约束度
                normalDir.y = normalDir.y * _VerticalBillboarding;
                normalDir = normalize(normalDir);
                
                // 得到三个正交基
                // 防止法线与向上方向平行，对法线的y分量进行判断，以得到合适的向上方向
                float3 upDir = abs(normalDir.y) > 0.999?  float3(0, 0, 1):float3(0, 1, 0);
                // 右方向
                float3 rightDir = normalize(cross(upDir, normalDir));
                // 上方向
                upDir = normalize(cross(normalDir, rightDir));
                // 计算新的顶点位置，根据原始的位置相对于锚点的偏移量以及正交基矢量，计算得到新的顶点位置
                float3 centerOffs = v.vertex.xyz - center;
                // 书中原版的变换方法，看不懂
                // float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir.z * centerOffs.z;
                // 使用旋转矩阵进行变换，可以填充到行，然后把向量放前面即可
                float3x3 rotation = float3x3(rightDir, upDir, normalDir);
                float3 localPos = center + mul(centerOffs, rotation);
                v2f o;
                o.pos = UnityObjectToClipPos(float4(localPos, 1));
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                c.rgb *= _Color.rgb;
                return c;
            }
            
            ENDCG
        }
    }
    Fallback Off
}