Shader "ShaderLearn/MotionBlur"
{
    Properties
    {
        _MainTex ("Base(RGB)", 2D) = "white" {}
        // blurAmount的值越大，运动拖尾的效果越明显，0~0.9范围内
        _BlurAmount("Blur Amount", Float) = 1.0
    }
    
    SubShader
    {
        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        fixed _BlurAmount;
        
        struct v2f
        {
            float4 pos:SV_POSITION;
            half2 uv:TEXCOORD0;
        };
        
        v2f vert(appdata_img v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord;
            return o;
        }
        
        // RGB部分片元
        fixed4 fragRGB(v2f i):SV_Target
        {
            return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
        }
        
        // A通道片元
        fixed4 fragA(v2f i):SV_Target
        {
            return tex2D(_MainTex, i.uv);
        }
         
        ENDCG
        ZTest Always
        Cull Off                                   
        ZWrite Off
        
        // 混合RGB通道
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRGB
            ENDCG
        }
        
        // 混合A通道
        Pass
        {
            Blend One Zero
            ColorMask A
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragA
            ENDCG
        }
    }
    FallBack Off
}
