using UnityEngine;
using System.Collections;

public class MotionBlurWithDepthTexture : PostEffectsBase {

	public Shader motionBlurShader;
	private Material motionBlurMaterial = null;

	private Material material {  
		get {
			motionBlurMaterial = CheckShaderAndCreateMaterial(motionBlurShader, motionBlurMaterial);
			return motionBlurMaterial;
		}  
	}
	
	// 需要得到摄像机的视角和投影矩阵，定义一个Camera类型的变量，以获取该脚本所在的摄像机组件
	private Camera myCamera;
	private new Camera camera {
		get {
			if (myCamera == null) {
				myCamera = GetComponent<Camera>();
			}
			return myCamera;
		}
	}

	// 定义运动模糊时模糊图像使用的大小
	[Range(0.0f, 1.0f)]
	public float blurSize = 0.5f;
	
	// 定义一个变量来保存上一帧摄像机的视角 * 投影矩阵
	private Matrix4x4 previousViewProjectionMatrix;

	private void OnEnable() {
		// 在脚本的OnEnable函数中设置摄像机的状态，用于获取摄像机的深度纹理
		camera.depthTextureMode |= DepthTextureMode.Depth;
		previousViewProjectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
	}
	
	// 首先需要计算和传递运动模糊使用的各个属性。
	// 本例需要使用两个变换矩阵——前一帧的视角 * 投影矩阵以及当前帧的视角 * 投影矩阵的逆矩阵。
	private void OnRenderImage (RenderTexture src, RenderTexture dest) {
		
		if (material != null) {
			material.SetFloat("_BlurSize", blurSize);
			//上一帧矩阵存储的矩阵
			material.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
			// 投影矩阵 * 世界转相机矩阵
			var currentViewProjectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
			// 取逆矩阵，得到投影空间 -> 世界空间的转换矩阵
			var currentViewProjectionInverseMatrix = currentViewProjectionMatrix.inverse;
			// 设置本帧逆矩阵传递给材质
			material.SetMatrix("_CurrentViewProjectionInverseMatrix", currentViewProjectionInverseMatrix);
			previousViewProjectionMatrix = currentViewProjectionMatrix;
			Graphics.Blit (src, dest, material);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
