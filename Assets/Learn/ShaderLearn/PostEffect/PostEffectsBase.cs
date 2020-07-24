using UnityEngine;
using System.Collections;

//要求所有屏幕后处理效果都需要绑定在某个摄像机上，并且在编辑器状态下也可以执行该脚本来查看效果
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{

    //start 中检测
    protected void CheckResources()
    {
        bool isSupported = CheckSupport();

        if (isSupported == false)
        {
            NotSupported();
        }
    }

    //检查当前平台是否支持屏幕后处理
    protected bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            Debug.LogWarning("This platform does not support image effects.");
            return false;
        }

        return true;
    }

    // 不支持则直接不可见
    protected void NotSupported()
    {
        enabled = false;
    }

    protected void Start()
    {
        CheckResources();
    }

    //参数：shader该特效需要使用的shader，material是用于后期处理的材质。检查shader可用通过返回第一个使用该shader的材质，否则返回null
    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            return null;
        }

        if (shader.isSupported && material && material.shader == shader)
            return material;

        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }
}
