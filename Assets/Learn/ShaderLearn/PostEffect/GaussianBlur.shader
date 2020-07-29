﻿// 高斯模糊
Shader "ShaderLearn/GaussianBlur"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BlurSize("Blur Size", Float) = 1.0
    }
    SubShader
    {
        // 类似C++中头文件功能，避免在子Pass中些完全一样的代码
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        float _BlurSize;
        
        struct v2f 
        {
            float4 pos:SV_POSITION;
            half2 uv[5]:TEXCOORD0;   
        };
        
        // 竖直方向顶点着色器
        v2f vertBlurVertical(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
	        o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
	        o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) *_BlurSize;
            return o;
        }
        
        // 水平方向顶点着色器
        v2f vertBlurHorizontal(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            half2 uv = v.texcoord;
            o.uv[0] = uv;
            o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
	        o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
	        o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            return o;
        }
        
        // 通用片元着色器
        fixed4 fragBlur(v2f i):SV_Target
        {
            float weight[3] = { 0.4026, 0.2442, 0.0545 };
            fixed3 sum = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
	        for (int idx = 1; idx < 3; idx++) 
	        {
		        sum += tex2D(_MainTex, i.uv[idx * 2 - 1]).rgb * weight[idx];
		        sum += tex2D(_MainTex, i.uv[idx * 2]).rgb * weight[idx];
            }
            return fixed4(sum, 1.0);
        }
        ENDCG
        
        ZTest Always
        Cull Off
        ZWrite Off
        
        Pass
        {
            NAME "GAUSSIAN_BLUR_VERTICAL"
            CGPROGRAM
            #pragma vertex vertBlurVertical
            #pragma fragment fragBlur
            ENDCG
        }

         Pass
        {
            NAME "GAUSSIAN_BLUR_HORIZONTAL"
            CGPROGRAM
            #pragma vertex vertBlurHorizontal
            #pragma fragment fragBlur
            ENDCG
        }
    }
    FallBack Off
}