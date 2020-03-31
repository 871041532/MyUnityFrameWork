using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlur : PostEffectsBase {
    public Shader motionBlurShader;
    private Material motionBlurMaterial = null;
    public Material material
    {
        get {
            motionBlurMaterial = CheckShaderAndCreateMaterial(motionBlurShader,motionBlurMaterial);
            return motionBlurMaterial;
        }
    }
    //定义运动模糊在混合图像时使用的模糊参数
    [Range(0.0f, 0.9f)]
    public float blurAmount = 0.5f;

    //定义一个RenderTexture类型的变量，保存之前图像叠加的结果
    private RenderTexture accumulationTexture;

    //当OnDisable时立即销毁accumulationTexture，希望在下一次开始应用运动模糊时重新叠加图像
    private void OnDisable()
    {
        DestroyImmediate(accumulationTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material!=null)
        {
            if (accumulationTexture==null || accumulationTexture.width!=src.width || accumulationTexture.height!=src.height)
            {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width,src.height,0);
                //设置为HideAndDontSave，表示这个变量不会显示在Hierarchy中，也不会保存到场景中
                accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(src, accumulationTexture);
            }
            //表明需要进行一个渲染纹理的恢复操作，表示Blit时不清空。恢复操作发生在渲染到纹理而该纹理又没有被提前清空或销毁的情况下。
            accumulationTexture.MarkRestoreExpected();
            material.SetFloat("_BlurAmount",1.0f-blurAmount);
            //混合
            Graphics.Blit(src, accumulationTexture, material);
            //传给目标纹理
            Graphics.Blit(accumulationTexture, dest);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
