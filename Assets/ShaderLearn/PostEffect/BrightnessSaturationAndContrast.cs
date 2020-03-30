using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//亮度、饱和度、对比度
public class BrightnessSaturationAndContrast : PostEffectsBase {
    public Shader briSatConShader; //指定的shader
    private Material briSatConMaterial; //创建的材质
    public Material material
    {
        get {
            briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader,briSatConMaterial);
            return briSatConMaterial;
        }
    }
    //调整亮度、饱和度和对比度的参数
    [Range(0.0f, 3.0f)]
    public float brightness = 1.0f;
    [Range(0.0f, 3.0f)]
    public float saturation = 1.0f;
    [Range(0.0f, 3.0f)]
    public float contrast = 1.0f;

    //每当OnRenderImage函数被调用时，它会检查材质是否可用。可用就把参数传递给材质，再调用Graphics.Blit进行处理；否则，直接把原图像显示到屏幕上，不做任何处理。
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetFloat("_Brightness",brightness);
            material.SetFloat("_Saturation",saturation);
            material.SetFloat("_Contrast",contrast);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
