using UnityEngine;
using System.Collections;

public class EdgeDetectNormalsAndDepth : PostEffectsBase {

	public Shader edgeDetectShader;
	private Material edgeDetectMaterial = null;
	public Material material {  
		get {
			edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
			return edgeDetectMaterial;
		}  
	}

	[Range(0.0f, 1.0f)]
	public float edgesOnly = 0.0f;  // 只有边缘

	public Color edgeColor = Color.black;  // 边缘颜色

	public Color backgroundColor = Color.white;  // 背景颜色

	public float sampleDistance = 1.0f;  // 采样距离，从视觉上来看，sampleDistance值越大，描边越宽。

	public float sensitivityDepth = 1.0f;  // 深度敏感度，当邻域的深度值相差多少时，会被认为存在一条边界。

	public float sensitivityNormals = 1.0f;  // 法线敏感度，当邻域的法线值相差多少时，会被认为存在一条边界。
	
	void OnEnable() {
		// 需要获取摄像机的深度+法线纹理，我们在脚本的OnEnable函数中设置摄像机的相应状态
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	// 添加ImageEffectOpaque属性，希望在不透明的Pass执行完毕后立即调用该函数，而不对透明物体产生影响
	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		// 把各个参数传递给材质
		if (material != null) {
			material.SetFloat("_EdgeOnly", edgesOnly);
			material.SetColor("_EdgeColor", edgeColor);
			material.SetColor("_BackgroundColor", backgroundColor);
			material.SetFloat("_SampleDistance", sampleDistance);
			material.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));

			Graphics.Blit(src, dest, material);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
