using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianBlur : PostEffectsBase
{
    public Shader gaussianBlurShader;
    private Material gaussianBlurMaterial = null;

    public Material material
    {
        get
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }

    //高斯模糊迭代次数
    [Range(0, 4)] public int iterations = 3;

    //模糊范围，blurSpread越大模糊程度越高，但采样却不会受到影响
    [Range(0.2f, 10.0f)] public float blurSpread = 0.6f;

    //缩放系数，downSample越大，需要处理的像素数越少，同时也能进一步提高模糊程度，但值过大会使图像像素化
    [Range(1, 20)] public int downSample = 2;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            // 利用缩放对图像进行降采样，从而减少需要处理的像素个数
            // 我们在声明缓冲区的大小时，使用了小于原屏幕分辨率的尺寸，并将该临时渲染纹理的滤波模式设置为双线性。这样，在调用第一个Pass时，我们需要处理的像素个数就是原来的几分之一。对图像进行降采样不仅可以减少需要处理的像素个数，提高性能，而且适当的降采样往往还可以得到更好的模糊效果。尽管downSample值越大，性能越好，但过大的downSample可能会造成图像像素化。
            int rtW = src.width / downSample;
            int rtH = src.height / downSample;
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            buffer0.filterMode = FilterMode.Bilinear;
            Graphics.Blit(src, buffer0);
            // 进行多次高斯模糊
            for (int i = 0; i < iterations; i++)
            {
                material.SetFloat("_BlurSize", 1.0f + i * blurSpread);
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0); 
                // 竖直方向滤波，Render the vertical pass
                Graphics.Blit(buffer0, buffer1, material, 0);            
                RenderTexture.ReleaseTemporary(buffer0);            
                buffer0 = buffer1;            
                buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);           
                // 水平方向滤波：Render the horizontal pass
                Graphics.Blit(buffer0, buffer1, material, 1);            
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}