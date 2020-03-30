// 后处理，亮度，饱和度，对比度
Shader "ShaderLearn/BrightnessSaturationAndContrast"
{
    // 不必须定义属性，属性定义只是显示在材质面板中，对于屏幕特效来说使用的材质都是临时创建的，不需要调参数
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Brightness("Brightness", Range(0, 3)) = 1
        _Saturation("Saturation", Range(0, 3)) = 1
        _Contrast("Contrast", Range(0, 3)) = 1
    }
    
    SubShader
    {
        pass{
        // 总是关闭深度写入，防止它挡住在其后面被渲染的物体，标准设置
        ZTest Always Cull Off ZWrite Off
        CGPROGRAM
        #include "UnityCG.cginc"
        #pragma vertex vert
        #pragma fragment frag       
        
        sampler2D _MainTex;
        half _Brightness;
        half _Saturation;
        half _Contrast;
        
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
        
        fixed4 frag(v2f i):SV_Target
        {
            fixed4 renderTex = tex2D(_MainTex, i.uv);
            // 亮度调整
            fixed3 finalColor = renderTex.rgb * _Brightness;
            // 饱和度调整(黑白效果)
            fixed luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
            fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
            finalColor = lerp(luminanceColor, finalColor, _Saturation);
            // 对比度调整
            fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
            finalColor = lerp(avgColor, finalColor, _Contrast);
            return fixed4(finalColor, renderTex.a);
        }
        ENDCG
      }
    }
    FallBack Off
}
