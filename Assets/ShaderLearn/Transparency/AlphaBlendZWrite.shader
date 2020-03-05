// 带深度写入的透明度混合
Shader "ShaderLearn/AlphaBlendZWrite"
{
    Properties
    {
        m_Color("Color Tint", Color) = (1, 1, 1, 1)
        m_MainTex("Main Tex", 2D) = "white"{}
    }
    
    SubShader
    {
       Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
       Pass {
       
       }
       Pass{
       } 
    }
    Fallback Off
}