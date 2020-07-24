Shader "ShaderLearn/ImageSequenceAnimation"
{
    Properties
    {
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex("Main Tex", 2D) = "white" {}
        _HorizontalAmount("Horizontal Amount", Float) = 8
        _VerticalAmount("Vertical Amount", Float) = 8
        _Speed("Speed", Range(1, 100)) = 30
    }
    
    SubShader
    {
        Pass
        {   
            //序列帧通常是透明纹理，需要设置Pass的相关状态以及混合
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
        
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _HorizontalAmount;
            float _VerticalAmount;
            float _Speed;
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target               
            {
                //把时间和速度相乘，floor得到模拟整数时间
                float time = floor(_Time.y * _Speed);
                // 第x行
                float row = floor(time / _HorizontalAmount);
                // 第x列
                float column = floor(time - row * _HorizontalAmount);
                
                half2 uv = i.uv + half2(column, -row);
                
                // 缩放映射
                uv.x /= _HorizontalAmount;
                uv.y /= _VerticalAmount;
                
                fixed4 color = tex2D(_MainTex, uv);
                color.rgb *= _Color;
                return color;
            }
            
            ENDCG
        }
    }
    Fallback Off
}