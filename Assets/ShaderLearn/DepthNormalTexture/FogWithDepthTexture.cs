using UnityEngine;
using System.Collections;

public class FogWithDepthTexture : PostEffectsBase {

	public Shader fogShader;
	private Material fogMaterial = null;
	private Material material {  
		get {
			fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
			return fogMaterial;
		}  
	}

	private Camera myCamera;
	private new Camera camera {
		get {
			if (myCamera == null) {
				myCamera = GetComponent<Camera>();
			}
			return myCamera;
		}
	}

	private Transform myCameraTransform;

	private Transform cameraTransform {
		get {
			if (myCameraTransform == null) {
				myCameraTransform = camera.transform;
			}

			return myCameraTransform;
		}
	}

	[Range(0.0f, 3.0f)]
	public float fogDensity = 1.0f;  // 控制雾的浓度

	public Color fogColor = Color.white;  // 控制雾的颜色

	public float fogStart = 0.0f;  // 控制雾的起始高度
	public float fogEnd = 2.0f;  // 控制雾的终止高度

	void OnEnable() {
		camera.depthTextureMode |= DepthTextureMode.Depth;
	}
	
	// 每帧都通过摄像机视口参数算出视口四个向量，传给shader
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (material != null) {
			Matrix4x4 frustumCorners = Matrix4x4.identity;  // 单位矩阵

			float fov = camera.fieldOfView;
			float near = camera.nearClipPlane;
			float aspect = camera.aspect;

			float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
			Vector3 toRight = cameraTransform.right * halfHeight * aspect;
			Vector3 toTop = cameraTransform.up * halfHeight;

			Vector3 topLeft = cameraTransform.forward * near + toTop - toRight;
			float scale = topLeft.magnitude / near;

			topLeft.Normalize();
			topLeft *= scale;

			Vector3 topRight = cameraTransform.forward * near + toRight + toTop;
			topRight.Normalize();
			topRight *= scale;

			Vector3 bottomLeft = cameraTransform.forward * near - toTop - toRight;
			bottomLeft.Normalize();
			bottomLeft *= scale;

			Vector3 bottomRight = cameraTransform.forward * near + toRight - toTop;
			bottomRight.Normalize();
			bottomRight *= scale;

			// 求出四个角的向量
			frustumCorners.SetRow(0, bottomLeft);
			frustumCorners.SetRow(1, bottomRight);
			frustumCorners.SetRow(2, topRight);
			frustumCorners.SetRow(3, topLeft);

			material.SetMatrix("_FrustumCornersRay", frustumCorners);  // 矩阵

			material.SetFloat("_FogDensity", fogDensity);  // 雾浓度
			material.SetColor("_FogColor", fogColor);  // 雾颜色
			material.SetFloat("_FogStart", fogStart);  // 起始高度
			material.SetFloat("_FogEnd", fogEnd);  // 终止高度

			Graphics.Blit (src, dest, material);
		} else {
			Graphics.Blit(src, dest);
		}
	}
}
