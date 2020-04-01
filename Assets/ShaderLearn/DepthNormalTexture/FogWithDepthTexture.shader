Shader "ShaderLearn/FogWithDepthTexture"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}  //renderTexture
        _FogDensity("Fog Density", Float) = 1.0
        _FogColor("Fog COlor", Color) = (1, 1, 1, 1)
        _FogStart("Fog Start", Float) = 0.0
        _FogEnd("Fog End", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        ENDCG
    }
    FallBack "Diffuse"
}
