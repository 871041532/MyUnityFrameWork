// 切线空间下法线
Shader "ShaderLearn/Dissolve"
{
    Properties
    {
        _BurnAmount("Burn Amount", Range(0, 1.0)) = 0  // 控制消融程度，值为0物体正常，为1完全消融
        _LineWidth("Burn Line Width", Range(0, 0.2)) = 0.1  // 烧焦线宽，值越大，火焰边缘蔓延范围越广
        _MainTex("Base(RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BurnFirstColor("Burn First Color", Color) = (1, 0.83, 0.26, 1)  // 火焰边缘颜色1
        _BurnSecondColor("Burn Second Color", Color) = (0.04, 1, 0.99, 1)  // 火焰边缘颜色2
        _BurnMap("Burn Map", 2D) = "white" {}  // 噪声纹理
    }
    SubShader
    {
        // 第一个pass做消融效果
        Pass{
        Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
        Cull Off  // 用Cull命令关闭了该Shader的面片剔除，也就是说，模型的正面和背面都会被渲染。这是因为，消融会导致裸露模型内部的构造，如果只渲染正面会出现错误的结果
        CGPROGRAM
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"
        #pragma multi_compile_fwdbase
        #pragma vertex vert
        #pragma fragment frag
        float _BurnAmount;
        float _LineWidth;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        sampler2D _BumpMap;
        float4 _BumpMap_ST;
        fixed3 _BurnFirstColor;
        fixed3 _BurnSecondColor;
        sampler2D _BurnMap;
        float4 _BurnMap_ST;
          
        struct v2f 
        {
            float4 pos:SV_POSITION;
            float2 uvMainTex:TEXCOORD0;
            float2 uvBumpMap:TEXCOORD1;
            float2 uvBurnMap:TEXCOORD2;
            float3 lightDir:TEXCOORD3;
            float3 worldPos:TEXCOORD4;
            SHADOW_COORDS(5)
        };
        
        v2f vert(appdata_tan v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uvMainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.uvBumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
            o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);
            // 光源方向从模型空间变换到了切线空间
            TANGENT_SPACE_ROTATION;
            o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            TRANSFER_SHADOW(o);  // 阴影
            return o;    
        }
        
        fixed4 frag(v2f i):SV_Target
        {
            // 首先对噪声纹理进行采样，并将采样结果和用于控制消融程度的属性_BurnAmount相减，传递给clip函数
            fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
            // 当结果小于0时，该像素将会被剔除，从而不会显示到屏幕上。如果通过了测试，则进行正常的光照计算
            clip(burn.r - _BurnAmount);
            
            // 计算环境光
            fixed3 albedo = tex2D(_MainTex, i.uvMainTex).rgb;
            fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
            // 漫反射
            float3 tangentLightDir = normalize(i.lightDir);
            fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uvBumpMap));
            fixed3 diffuse = _LightColor0.rgb * albedo * saturate(dot(tangentNormal, tangentLightDir)); 
            
            // 在宽度为_LineWidth（比例）的范围内模拟一个烧焦的颜色变化 
            // 第一步使用了smoothstep函数来计算混合系数t。当t值为1时，表明该像素位于消融的边界处，当t值为0时，表明该像素为正常的模型颜色，而中间的插值则表示需要模拟一个烧焦效果。
            // 首先用t来混合两种火焰颜色_BurnFirstColor和_BurnSecond-Color，为了让效果更接近烧焦的痕迹，我们还使用pow函数对结果进行处理
            // 然后，再次使用t来混合正常的光照颜色（环境光+漫反射）和烧焦颜色。我们这里又使用了step函数来保证当_BurnAmount为0时，不显示任何消融效果。最后，返回混合后的颜色值finalColor。 
            fixed t = 1 - smoothstep(0, _LineWidth, burn.r - _BurnAmount);
            fixed3 burnColor = lerp(_BurnFirstColor, _BurnSecondColor, t);  
            burnColor = pow(burnColor, 5);
            
            UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
            fixed3 finalColor = lerp(ambient + diffuse * atten, burnColor, t * step(0.0001, _BurnAmount));
            return fixed4(finalColor, 1);
        }
        ENDCG
        }
        
        // 投射阴影的Pass
        // 阴影投射的重点在于需要按正常Pass的处理来剔除片元或进行顶点动画，以便阴影可以和物体正常渲染的结果相匹配。
        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}
            CGPROGRAM
            #pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			fixed _BurnAmount;
			sampler2D _BurnMap;
			float4 _BurnMap_ST;
            struct v2f 
            {
                V2F_SHADOW_CASTER;
                float2 uvBurnMap:TEXCOORD1;
            };
            
            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);
                return o;
            }
            
            fixed4 frag(v2f i):SV_Target
            {
                fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
                clip(burn.r - _BurnAmount);
                SHADOW_CASTER_FRAGMENT(i)    
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
