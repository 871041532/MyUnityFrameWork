﻿// 边缘检测：使用Sobel算子进行卷积
Shader "ShaderLearn/EdgeDetection"
{
    Properties
    {
        _MainTex("Base(RGB)", 2D) = "white" {}
        _EdgeOnly("_EdgeOnly", Range(0, 1)) = 1
        _EdgeColor("_EdgeColor", Color) = (1, 1, 1, 1)
        _BackgroundColor("_BackgroundColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _MainTex;
            //由于卷积需要对相邻区域内的纹理进行采样，因此需要利用_MainTex_TexelSize来计算各个相邻区域的纹理坐标
            half4 _MainTex_TexelSize;
            fixed _EdgeOnly;
            fixed4 _EdgeColor;
            fixed4 _BackgroundColor;
            
            struct v2f
            {
                float4 pos:SV_POSITION;
                half2 uv[9]:TEXCOORD0;
            };
           
            fixed luminance(fixed4 color)
	        {
		        return 0.2125*color.r + 0.7154*color.g + 0.0721*color.b;
	        }
	        
	        half Sobel(v2f i)
	        {
		        const half Gx[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1};
		        const half Gy[9] = {-1, 0,1,-2, 0, 2, -1, 0, 1};
		        half texColor;
	            half edgeX = 0;
	            half edgeY = 0;
	            for (int it = 0; it < 9; ++it)
	            {
		            texColor = luminance(tex2D(_MainTex, i.uv[it]));
		            edgeX += texColor * Gx[it];
		            edgeY += texColor * Gy[it];
	            }
	            half edge = 1 - abs(edgeX) - abs(edgeY);
	            return edge;
            }

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                half2 uv = v.texcoord;
                o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
		        o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
		        o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
		        o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
		        o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
		        o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
		        o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
		        o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                //调用Sobel函数，利用Sobel算子对原图进行边缘检测，计算当前像素的梯度值edge
                half edge = Sobel(i);
                //使用edge计算背景为原图和纯色下的颜色值
                fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[4]), edge);
                fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
                // 用_EdgeOnly在两者之间插值得到最终的像素值
                return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
            }
            ENDCG
        }
    }
    FallBack Off
}