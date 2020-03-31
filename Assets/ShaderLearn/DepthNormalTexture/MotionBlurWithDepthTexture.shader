 // 基于深度纹理的运动模糊
Shader "Custom/MotionBlurWithDepthTexture"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlurSize("Blur Size", Float)=1.0
    }
    
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        sampler2D _CameraDepthTexture;  // 深度纹理
        float4x4 _CurrentViewProjectionInverseMatrix;
        float4x4 _PreviousViewProjectionMatrix;
        half _BlurSize;
        
        struct v2f 
        {
            float4 pos:SV_POSITION;
            half2 uv:TEXCOORD0;
            half2 uv_depth:TEXCOORD1;  // 专门对深度纹理进行纹理采样
        };
        
        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            o.uv_depth = v.texcoord;
            // 对深度纹理的采样坐标进行了平台差异化处理，以便在类似DirectX的平台上，在开启了抗锯齿的情况下仍然可以得到正确的结果
            #if UNITY_UV_START_AT_TOP
            if (_MainTex_TexelSize.y < 0)
            {
                o.uv_depth.y = 1 - o.uv_depth.y;   
            }
            #endif
            return o;
        }
        
        fixed4 frag(v2f i):SV_Target
        {
            // 获取深度值
            float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
            // 通过深度值得到NDC坐标
            float4 H = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, d * 2 - 1, 1);
            // 通过矩阵将NDC坐标变换到世界空间下
            float4 D = mul(_CurrentViewProjectionInverseMatrix, H);
            float4 worldPos = D / D.w;
                                                           
            // 最终获取上一帧的NDC坐标
            float4 currentPos = H;
            float4 previousPos = mul(_PreviousViewProjectionMatrix, worldPos);
            previousPos /= previousPos.w;
            // 获取像素速度值
            float2 velocity = (currentPos.xy - previousPos.xy) / 2.0f;
            
            float2 uv = i.uv;
            float4 c = tex2D(_MainTex, uv);
            // 使用该速度值对它的邻域像素进行采样，相加后取平均值得到一个模糊的效果。
            uv += velocity * _BlurSize;
            for (int idx = 1; idx < 3; ++idx, uv += velocity * _BlurSize)
            {
                float4 currentColor = tex2D(_MainTex, uv);
                c += currentColor;    
            }
            c /= 3;
            return fixed4(c.rgb, 1.0);
        }
        ENDCG
        
        Pass
        {
            ZTest Always  // 深度测试总是通过
            Cull Off  // 不剔除
            ZWrite Off  // 不写入深度缓存
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack Off
}
