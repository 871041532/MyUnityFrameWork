// 基于顶点动画的2D流水，从上往下流，左右摇晃
Shader "ShaderLearn/WaterPoint"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}  // 河流纹理
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _Magnitude("Distortion Magnitude", Float) = 1  // 水流变形幅度
        _Frequency("Distortion Frequency", Float) = 1  //  水流变形频率
        _InvWaveLength("Distortion Inverse Wave Length", Float) = 10  // 波长的倒数
        _Speed("Speed", Float) = 0.5  // 河流纹理移动速度
    }
    
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True"}
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Magnitude;
            float _Frequency;
            float _InvWaveLength;
            float _Speed;
            
            struct v2f 
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                float4 offset;
                offset.yzw = float3(0, 0, 0);
                offset.x = sin(_Frequency * _Time.y + v.vertex.x * _InvWaveLength + v.vertex.y * _InvWaveLength + v.vertex.z * _InvWaveLength) * _Magnitude;
                o.pos = UnityObjectToClipPos(v.vertex + offset);  // 此处进行左右变形
                
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv += float2(0, _Time.y * _Speed);  // 此处进行上下流动摇晃
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                color.rgb *= _Color.rgb;
                return color;
            }
            
            ENDCG
        }
    }
    
    Fallback Off
}