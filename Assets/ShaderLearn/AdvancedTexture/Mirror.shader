// 镜子效果
Shader "ShaderLearn/Mirror"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "whirte" {}
    }
    SubShader
    {
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD2;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.uv.x = 1 - o.uv.x;
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            
            ENDCG
        }
    }
    Fallback "Specular" 
}