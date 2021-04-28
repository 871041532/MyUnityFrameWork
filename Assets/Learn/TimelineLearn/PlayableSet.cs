// 这个脚本可以运行timeline

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var playable = gameObject.GetComponent<PlayableDirector>();
        var light = gameObject.GetComponent<Light>();
        var bindingDict = new Dictionary<string, PlayableBinding>();

        foreach (var bind in playable.playableAsset.outputs)
        {
            bindingDict.Add(bind.streamName, bind);
        }
        
        // 动态绑定轨道中的节点 SetGenericBinding
        playable.SetGenericBinding(bindingDict["LightControlTrack"].sourceObject, light);
        
        // 动态绑定轨道中clip的节点 SetReferenceValue
        foreach (var track in ((TimelineAsset)playable.playableAsset).GetOutputTracks())
        {
            foreach (var clip in track.GetClips())
            {
                LightControlAsset asset = clip.asset as LightControlAsset;
                playable.SetReferenceValue(asset.light.exposedName, light);
            }
        }
        
        playable.Play();
    }
}