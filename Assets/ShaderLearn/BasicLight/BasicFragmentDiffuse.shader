// 逐像素漫反射和半兰伯特
Shader "ShaderLearn/BasicFragmentDiffuse"
{
    Properties{
    	m_DiffuseColor("Diffuse Color", Color) = (1,1,1,1)
    }

    SubShader{
    	Pass{
    		Tags{"LightMode" = "ForwardBase"}
    		CGPROGRAM
    		#include "UnityCG.cginc"
    		#include "Lighting.cginc"
    		#pragma vertex vert
    		#pragma fragment frag 
    		fixed4 m_DiffuseColor;
    		
    		struct v2f {
    		    float4 pos:SV_POSITION;
    		    float3 worldNormal:TEXCOORD0;
    		};
    		
    		v2f vert(appdata_base v) {
    		     v2f  o;
    		     o.pos = UnityObjectToClipPos(v.vertex);
    		     o.worldNormal = UnityObjectToWorldNormal(v.normal);
    		     return o;
    		}
    		
    		 fixed4 frag(v2f i):SV_Target {
    		      fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
    		      fixed3 worldNormal = normalize(i.worldNormal);
    		      // 光源方向
    		      fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
//    		      fixed3 diffuse = _LightColor0.rgb * m_DiffuseColor.rgb * saturate(dot(worldNormal, worldLight));
    		      // 半兰伯特
    		      fixed3 diffuse = _LightColor0.rgb * m_DiffuseColor.rgb * (0.5 * dot(worldNormal, worldLight) + 0.5);
    		      return fixed4(ambient + diffuse, 1.0);
    		 }
    		ENDCG
    	}
    }
     Fallback off
}