Shader "ShaderLearn/SurfaceExpand" {
	Properties {
		_ColorTint ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Amount ("Extrusion Amount", Range(-0.5, 0.5)) = 0.1
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
        // 在#pragma surface的编译指令一行中还指定了一些额外的参数。
        // 由于修改了顶点位置，因此，要对其他物体产生正确的阴影效果并不能直接依赖FallBack中找到的阴影投射Pass，addshadow参数可以告诉Unity要生成一个该表面着色器对应的阴影投射Pass。默认情况下，Unity会为所有支持的渲染路径生成相应的Pass，为了缩小自动生成的代码量，我们使用exclude_path:deferred和exclude_path:prepass来告诉Unity不要为延迟渲染路径生成相应的Pass。
        // 使用nometa参数取消对提取元数据的Pass的生成。
		#pragma surface surf CustomLambert vertex:myvert finalcolor:mycolor addshadow exclude_path:deferred exclude_path:prepass nometa
		#pragma target 3.0
		
		fixed4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Amount;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};
		
		// 在顶点修改函数中，我们使用顶点法线对顶点位置进行膨胀
		void myvert (inout appdata_full v) {
			v.vertex.xyz += v.normal * _Amount;
		}
		
		// 表面函数使用主纹理设置了表面属性中的反射率，并使用法线纹理设置了表面法线方向
		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		
		// 光照函数实现了简单的兰伯特漫反射光照模型
		half4 LightingCustomLambert (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
			c.a = s.Alpha;
			return c;
		}
		
		// 在最后的颜色修改函数中，简单地使用了颜色参数对输出颜色进行调整
		void mycolor (Input IN, SurfaceOutput o, inout fixed4 color) {
			color *= _ColorTint;
		}
		
		ENDCG
	}
	FallBack "Legacy Shaders/Diffuse"
}
