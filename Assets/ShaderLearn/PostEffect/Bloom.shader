// 基于高斯模糊的bloom效果
Shader "ShaderLearn/Bloom" {
        Properties{
            _MainTex("Base (RGB)", 2D) = "white" {}
            _BlurSize("Blur Size", Float) = 1.0
            _Bloom("Bloom (RGB)",2D) = "black"{}
            //阈值0~1，0表示全部是亮区域，1表示全是暗区域
            _LuminanceThreshold("Luminance Threashold",Float)=0.5
        }

        SubShader{
            CGINCLUDE
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            sampler2D _Bloom;
            float _LuminanceThreshold;
            float _BlurSize;

            //1.定义提取较亮区域需要使用的顶点着色器和片元着色器
        struct v2f {
            float4 pos : SV_POSITION;
            half2 uv: TEXCOORD0;
        };

        v2f vertExtractBright(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            return o;
        }
        fixed luminance(fixed4 color)
        {
            return 0.2125*color.r + 0.7154*color.g + 0.0721*color.b;
        }

        fixed4 fragExtractBright(v2f i):SV_Target
        {
            fixed4 c = tex2D(_MainTex,i.uv);
            //采样得到的亮度值减去阈值
            fixed val = clamp(luminance(c)-_LuminanceThreshold,0,1);
            return c * val;
        }

        //2.定义混合亮部图像和原图像时使用的顶点着色器和片元着色器
        struct v2fBloom
        {
            float4 pos:SV_POSITION;
            half4 uv:TEXCOORD0;
        };

        v2fBloom vertBloom(appdata_img v)
        {
            v2fBloom o;

            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv.xy = v.texcoord;
            o.uv.zw = v.texcoord;
        
#if UNITY_UV_STARTS_AT_TOP //差异化平台
            if (_MainTex_TexelSize.y<0)
            {
                o.uv.w = 1.0 - o.uv.w;
            }
#endif
            return o;
        }
        
        fixed4 fragBloom(v2fBloom i):SV_Target
        {
            return tex2D(_MainTex,i.uv.xy)+tex2D(_Bloom,i.uv.zw);
        }


        //3.高斯模糊所使用的着色器
        struct blurV2f {
            float4 pos : SV_POSITION;
            half2 uv[5]: TEXCOORD0;
        };
        
        //竖直方向顶点着色器
        blurV2f vertBlurVertical(appdata_img v) {
            blurV2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) *_BlurSize;
            return o;
        }
        
        //水平方向顶点着色器
        blurV2f vertBlurHorizontal(appdata_img v) {
            blurV2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            return o;
        }

        fixed4 fragBlur(blurV2f i) : SV_Target{
            float weight[3] = { 0.4026, 0.2442, 0.0545 };
            fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
            for (int it = 1; it < 3; it++) {
                sum += tex2D(_MainTex, i.uv[it * 2 - 1]).rgb * weight[it];
                sum += tex2D(_MainTex, i.uv[it * 2]).rgb * weight[it];
             }
            return fixed4(sum, 1.0);
        }
        ENDCG
        
        ZTest Always Cull Off ZWrite Off
        
        //第一个Pass提取较亮区域
        Pass{
            CGPROGRAM
            #pragma vertex vertExtractBright
            #pragma fragment fragExtractBright
            ENDCG
        }
        
        // 高斯模糊是非常常见的图像处理操作，很多屏幕特效都是建立在它的基础上的
        // 为Pass定义名字，可以在其他Shader中直接通过它们的名字来使用该Pass，而不需要再重复编写代码。
        //第二个Pass竖直方向高斯模糊
        Pass {
            NAME "GAUSSIAN_BLUR_VERTICAL"
            CGPROGRAM
            #pragma vertex vertBlurVertical  
            #pragma fragment fragBlur
            ENDCG
        }

        //第三个Pass水平方向高斯模糊
        Pass {
            NAME "GAUSSIAN_BLUR_HORIZONTAL"
            CGPROGRAM
            #pragma vertex vertBlurHorizontal  
            #pragma fragment fragBlur
            ENDCG
        }

        //第四个Pass混合效果
        Pass{
            CGPROGRAM
            #pragma vertex vertBloom
            #pragma fragment fragBloom
            ENDCG
        }
    }
    FallBack Off
}
