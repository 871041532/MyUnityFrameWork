using UnityEngine;

public class EdgeDetection : PostEffectsBase {
    public Shader edgeDetectShader;
    private Material edgeDetectMaterial = null;
    public Material material
    {
        get {
            edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader,edgeDetectMaterial);
            return edgeDetectMaterial;
        }
    }
    // 边缘线强度，值为0时，边缘将会叠加在原渲染图像上；值为1时，则只会显示边缘，不显示原渲染图像。
    // 其中，背景颜色由backgroundColor指定，边缘颜色由edgeColor指定
    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f;
    // 描边颜色
    public Color edgeColor = Color.black;
    // 背景颜色
    public Color backgroundColor = Color.white;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material!=null)
        {
            material.SetFloat("_EdgeOnly",edgesOnly);
            material.SetColor("_EdgeColor",edgeColor);
            material.SetColor("_BackgroundColor",backgroundColor);
            Graphics.Blit(source,destination,material);
        }
        else
        {
            Graphics.Blit(source,destination);
        }
    }
}
