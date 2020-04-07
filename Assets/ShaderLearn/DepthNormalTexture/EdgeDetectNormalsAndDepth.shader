Shader "ShaderLearn/EdgeDetectNormalsAndDepth"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EdgeOnly("Edge Only", Float) = 1.0
        _EdgeColor("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor("Background Color", Color) = (1, 1, 1, 1)
        _SampleDistance("Sample Distance", Float) = 1.0  // 采样距离，值越大描边越粗
        _Sensitivity("Sensitivity", Vector) = (1, 1, 1, 1)  // x法线灵敏度，y深度灵敏度
    }
    SubShader
    {

        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        half4 _MainTex_TexelSize; 
        fixed _EdgeOnly;
        fixed4 _EdgeColor;
        fixed4 _BackgroundColor;
        float _SampleDistance;
        half4 _Sensitivity;
        sampler2D _CameraDepthNormalsTexture;
        
        struct v2f 
        {
            float4 pos:SV_POSITION;
            half2 uv[5]:TEXCOORD0;
        };
        
        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            half2 uv = v.texcoord;
            // 第一个坐标存储了屏幕颜色图像的采样纹理, 在必要情况下对它的竖直方向进行了翻转
            o.uv[0] = uv;
            #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0)
                uv.y = 1- uv.y;
            #endif
            // 数组中剩余的4个坐标则存储了使用Roberts算子时需要采样的纹理坐标
            //使用了_SampleDis-tance来控制采样距离。通过把计算采样纹理坐标的代码从片元着色器中转移到顶点着色器中，可以减少运算，提高性能。 
            // 从顶点着色器到片元着色器的插值是线性的，因此这样的转移并不会影响纹理坐标的计算结果。
            o.uv[1] = uv + _MainTex_TexelSize.xy * half2(1, 1) * _SampleDistance;
            o.uv[2] = uv + _MainTex_TexelSize.xy * half2(-1, -1) * _SampleDistance;
            o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 1) * _SampleDistance;
            o.uv[4] = uv + _MainTex_TexelSize.xy * half2(1, -1) * _SampleDistance;
            return o;
        }
        
        // 返回0时表明这两点之间存在一条边界, 返回1表示不存在边界
        half CheckSame(half4 center, half4 sample)
        {
            // 首先对输入参数进行处理，得到两个采样点的法线和深度值。
            // 并没有解码得到真正的法线值，而是直接使用了xy分量。这是因为只需要比较两个采样值之间的差异度，而并不需要知道它们真正的法线值
            half2 centerNormal = center.xy;
            float centerDepth = DecodeFloatRG(center.zw);
            half2 sampleNormal = sample.xy;
            float sampleDepth = DecodeFloatRG(sample.zw);
            
            half2 diffNormal = abs(centerNormal - sampleNormal) * _Sensitivity.x;  
            int isSampleNormal = (diffNormal.x + diffNormal.y) < 0.1;
            
            float diffDepth = abs(centerDepth - sampleDepth) * _Sensitivity.y;
            int isSameDepth = diffDepth < 0.1 * centerDepth;
            return isSampleNormal * isSameDepth? 1.0 : 0.0;
        }
        
        fixed4 fragRobertsCrossDepthAndNormal(v2f i):SV_Target
        {
            // 首先使用4个纹理坐标对深度+法线纹理进行采样
            half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
            half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
            half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
            half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4]);
            // 再调用CheckSame函数来分别计算对角线上两个纹理值的差值
            // CheckSame函数的返回值要么是0，要么是1，返回0时表明这两点之间存在一条边界，反之则返回1
            half edge = 1.0;
            edge *= CheckSame(sample1, sample2);
            edge *= CheckSame(sample3, sample4);
            // 颜色混合
            fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
            fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
            return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
        }
        
        ENDCG
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRobertsCrossDepthAndNormal
            ENDCG
        }
    }
    FallBack Off
}