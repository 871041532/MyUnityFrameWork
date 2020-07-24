// 两个背景形成滚动卷轴效果，需要texture WrapMode设置成Repeat
Shader "ShaderLearn/ScrollingBackground"
{
    Properties
    {
        _MainTex("Base Layer(RGB)", 2D) = "white" {}
        _DetailTex("2nd Layer(RGB)", 2D) = "whire" {}
        _ScrollX("Base Layer Scroll Speed", Float) = 0.125
        _Scroll2X("2nd Layer Scroll Speed", Float) = 0.25
        _Multiplier("Layer _Multiplier", Float) = 1.0  // 控制亮度
    }
    
    SubShader
    {
        Pass 
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DetailTex;
            float4 _DetailTex_ST;
            float _ScrollX;
            float _Scroll2X;
            float _Multiplier;
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float4 uv:TEXCOORD0;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex) + float2(frac(_ScrollX * _Time.y), 0);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _DetailTex) + float2(frac(_Scroll2X * _Time.y), 0);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 firstLayer = tex2D(_MainTex, i.uv.xy);
                fixed4 secondLayer = tex2D(_DetailTex, i.uv.zw);
                fixed4 color = lerp(firstLayer, secondLayer, secondLayer.a);
                color.rgb *= _Multiplier;
                return color;
            }
            
            ENDCG
        }
    }
    
    Fallback Off
}