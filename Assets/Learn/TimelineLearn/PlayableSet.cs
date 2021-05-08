// 这个脚本可以运行timeline

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableSet : MonoBehaviour
{
    private PlayableDirector playable;

    private Light light;
    public Light light2;

    private Dictionary<string, PlayableBinding> bindingDict;

    public GameObject sphere;
    
    // Start is called before the first frame update
    void Start()
    {
        playable = gameObject.GetComponent<PlayableDirector>();
         light = gameObject.GetComponent<Light>();
         bindingDict = new Dictionary<string, PlayableBinding>();

        foreach (var bind in playable.playableAsset.outputs)
        {
            bindingDict.Add(bind.streamName, bind);
        }
        
        // 动态绑定轨道中的节点 SetGenericBinding
        playable.SetGenericBinding(bindingDict["LightTrack1"].sourceObject, light);
//        playable.SetGenericBinding(bindingDict["LightTrack2"].sourceObject, light);
        playable.SetGenericBinding(bindingDict["Animation"].sourceObject, sphere);
        
        // 动态绑定轨道中clip的节点 SetReferenceValue
        foreach (var track in ((TimelineAsset)playable.playableAsset).GetOutputTracks())
        {
            track.muted = false;
            foreach (var clip in track.GetClips())
            {
                LightControlAsset asset = clip.asset as LightControlAsset;
                if (asset!=null)
                {
                    playable.SetReferenceValue(asset.light.exposedName, light);
                }
            }
        }
        
        // 获取track信息
        TimelineAsset timelineAsset = (TimelineAsset) playable.playableAsset;
//        timelineAsset.DeleteTrack()
//        timelineAsset.A
        var count = timelineAsset.outputTrackCount;
        for (int i = 1; i < count; i++)
        {
            TrackAsset asset = timelineAsset.GetOutputTrack(i);
            var clips = asset.GetClips();
            foreach (TimelineClip clip in clips)
            {
                var playableAsset = (PlayableAsset)clip.asset;
//                var behaviout = playableAsset.behaviour;
            }
        }
        
        var graphCount = playable.playableGraph.GetOutputCount();;
        for (int i = 0; i < graphCount; i++)
        {
          
//            var playable1 = playable.playableGraph.GetRootPlayable(i);
            PlayableOutput output = playable.playableGraph.GetOutput(i);
            var p1 = output.GetSourcePlayable();
//            p1.GetInputCount();
            Playable i1 = p1.GetInput(1);
//            playable.playableAsset
//            i1.SetInputWeight(0);
            
//            output.SetWeight();
            var graph = playable.playableGraph;
//            graph
            var playable2 = output.GetSourcePlayable();
        }
        playable.Play();
    }

    private int index = 0;
    private void Update()
    {
        index++;
        if (index > 60)
        {
            var curTime = playable.time;
            foreach (var track in ((TimelineAsset) playable.playableAsset).GetOutputTracks())
            {
               
            }

//            playable.SetGenericBinding(bindingDict["Animation"].sourceObject, null);
//            playable.SetGenericBinding(bindingDict["LightTrack2"].sourceObject, null);
            playable.RebuildGraph();
            playable.time = curTime;
            index = -1000000;
        }
    }

    public void MuteTrack(TrackAsset track)
    {
        
    }
    private void OnDisable()
    {
        foreach (var track in ((TimelineAsset) playable.playableAsset).GetOutputTracks())
        {
            track.muted = false;
        }
    }
}