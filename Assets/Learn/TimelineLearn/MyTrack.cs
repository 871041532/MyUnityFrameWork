using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


// 逻辑
public class LightControlBehaviour : PlayableBehaviour
{
    // public Light light;
    public Color color = Color.white;
    public float intensity = 1;
    public ulong startFrameId = 0;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        startFrameId = info.frameId;
    }

    // 每次更新都会执行此方法
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log(string.Format("当前帧：{0}", info.frameId - startFrameId));
        // playerData通过playable.SetGenericBinding绑定设置过来
        Light light = playerData as Light;
        if (light != null)
        {
            light.color = color;
            light.intensity = intensity;
        }
    }
}

// 资源
public class LightControlAsset : PlayableAsset
{
    // 在clip中的light对象
    public ExposedReference<Light> light;
    public Color color = Color.white;
    public float intensity = 1;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // 把逻辑和资源联系起来
        var playable = ScriptPlayable<LightControlBehaviour>.Create(graph);
        var lightControlbehaviour = playable.GetBehaviour();
        // 获取到light的对象（通过playable.SetReferenceValue设置过来）
        var light2 = light.Resolve(graph.GetResolver());
//            lightControlbehaviour.light = light.Resolve(graph.GetResolver());
        lightControlbehaviour.color = color;
        lightControlbehaviour.intensity = intensity;
        return playable;
    }
}

// 轨道
[TrackClipType(typeof(LightControlAsset))] // 绑定的clip的类型
[TrackBindingType(typeof(Light))]
// 绑定的Track对象的类型
public class LightControlTrack : TrackAsset
{
}

