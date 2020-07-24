// 深度测试
Shader "ShaderLearn/AlphaTestShadow" 
{
    Properties{
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex("Main Tex", 2D) = "white"{}
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    
    SubShader{
        // Unity透明度测试渲染队列是AlphaTest的，需要把Queue标签设置为此。
        // IgnoreProjector设为True意味着这个Shader不会受到投影器的影响
        // RenderType标签通常被用于着色器替换功能，让Unity把这个Shader归入到提前定义的组中，以指明该shader使用了透明度测试的shader
        // 通常使用了透明度测试的shader都应该在SubShader中设置这三个标签
        Tags{"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        Pass{ 
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            struct v2f
            {
                float4 pos:SV_POSITION;
                float3 worldNormal:TEXCOORD0;
                float3 worldPos:TEXCOORD1;
                float2 uv:TEXCOORD2;
                SHADOW_COORDS(3)
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                TRANSFER_SHADOW(o);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPos));
                
                fixed4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _Cutoff);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLight));
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                return fixed4(diffuse * atten + ambient, 1.0);
            } 
            ENDCG
        }
    }
    Fallback "Transparent/Cutout/VertexLit"
}