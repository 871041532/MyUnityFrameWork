using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : PostEffectsBase {
    public Shader gaussianBlurShader;
    private Material gaussianBlurMaterial = null;
    public Material material
    {
        get {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader,gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }
    //高斯模糊迭代次数
    [Range(0,4)]
    public int iterations = 3;
    //模糊范围，blurSpread越大模糊程度越高，但采样却不会受到影响
    [Range(0.2f,10.0f)]
    public float blurSpread = 0.6f;
    //缩放系数，downSample越大，需要处理的像素数越少，同时也能进一步提高模糊程度，但值过大会使图像像素化
    [Range(1, 8)]
    public int downSample = 2;
    //控制提取较亮区域时使用的阈值, 尽管在绝大多数情况下，图像的亮度值不会超过1。但如果我们开启了HDR，硬件会允许我们把颜色值存储在一个更高精度范围的缓冲中，此时像素的亮度值可能会超过1。因此，在这里我们把luminanceThreshold的值规定在[0, 4]范围内。
    [Range(0.0f, 4.0f)]
    public float luminanceThreshold = 0.6f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material!=null)
        {
            material.SetFloat("_LuminanceThreshold",luminanceThreshold);
            int rtW = source.width/downSample;
            int rtH = source.height/downSample;
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW,rtH,0);
            buffer0.filterMode = FilterMode.Bilinear;

            //首先选出较亮区域存于buffer0
            Graphics.Blit(source, buffer0, material, 0);

            //对较亮区域进行多次高斯模糊
            if (iterations > 0)
            {
                RenderTexture tempbuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
                for (int i = 0; i < iterations; i++)
                {
                    material.SetFloat("_BlurSize",1.0f+i*blurSpread);  
                    //竖直方向滤波
                    Graphics.Blit(buffer0, tempbuffer, material, 1);
                    //水平方向滤波
                    Graphics.Blit(tempbuffer, buffer0, material, 2);               
                }
                RenderTexture.ReleaseTemporary(tempbuffer);       
            }
            //较亮区域写入Shader中_Bloom纹理
            material.SetTexture("_Bloom", buffer0);

            //最终效果，将source与buffer0相加混合
            Graphics.Blit(source, destination, material, 3);
            RenderTexture.ReleaseTemporary(buffer0); 
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
