// 这个脚本可以运行timeline

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayableSet : MonoBehaviour
{
    public float value = 0;
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
//        playable.SetGenericBinding(bindingDict["LightTrack1"].sourceObject, light);
        playable.SetGenericBinding(bindingDict["Animation1"].sourceObject, sphere);
        
        
        // 动态绑定轨道中clip的节点 SetReferenceValue
        foreach (var track in ((TimelineAsset)playable.playableAsset).GetOutputTracks())
        {
//            track.muted = false;
            foreach (var clip in track.GetClips())
            {
                LightControlAsset asset = clip.asset as LightControlAsset;
                if (asset!=null)
                {
                    playable.SetReferenceValue(asset.light.exposedName, light);
                }
                ControlPlayableAsset asset2 = clip.asset as ControlPlayableAsset;
                if (asset2 != null)
                {
                    playable.SetReferenceValue(asset2.sourceGameObject.exposedName, sphere);
                }
            }
        }
        playable.SetGenericBinding(bindingDict["ActivationTrack"].sourceObject, null);
//        playable.RebuildGraph();
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
                var lightAsset = playableAsset as LightControlAsset;
                if (lightAsset != null)
                {
//                    lightAsset.behaviour.OnGraphStop();
                }
//                var behaviout = playableAsset.behaviour;
            }
        }
        
//        var graphCount = playable.playableGraph.GetOutputCount();
//        for (int i = 0; i < graphCount; i++)
//        {
//          
////            var playable1 = playable.playableGraph.GetRootPlayable(i);
//            PlayableOutput output = playable.playableGraph.GetOutput(i);
//            var p1 = output.GetSourcePlayable();
////            p1.GetInputCount();
//            Playable i1 = p1.GetInput(1);
////            playable.playableAsset
////            i1.SetInputWeight(0);
//            
////            output.SetWeight();
//            var graph = playable.playableGraph;
////            graph
//            var playable2 = output.GetSourcePlayable();
//        }
        playable.Play();
    }

    private int index = 0;
    private void Update()
    {
        index++;
        if (index > 30)
        {
            var curTime = playable.time;
            foreach (var track in ((TimelineAsset) playable.playableAsset).GetOutputTracks())
            {    

              foreach (var clip in track.GetClips())
               {
                        LightControlAsset asset = clip.asset as LightControlAsset;
                        if (asset!=null)
                        {
                            playable.SetReferenceValue(asset.light.exposedName, null);
                        }
              }
                
                // 用修改绑定方法改变
                playable.SetGenericBinding(bindingDict["ActivationTrack"].sourceObject, null);
                if (track as AnimationTrack)
                {
                    playable.SetGenericBinding(track, null);
                }
                else if(track as LightControlTrack)
                {
//                    playable.SetGenericBinding(track, null);  
                }
                
                // 用暂停playableBehaviour方式实现
                var clips = track.GetClips();
                foreach (TimelineClip clip in clips)
                {
                    if (clip.asset as LightControlAsset)
                    {
                        var behaviour = (clip.asset as LightControlAsset).behaviour;
//                        behaviour.OnBehaviourPause();
                    }
                    ControlPlayableAsset asset2 = clip.asset as ControlPlayableAsset;
                    if (asset2 != null)
                    {
                        playable.SetReferenceValue(asset2.sourceGameObject.exposedName, null);
                    }
//                var behaviout = playableAsset.behaviour;
                }
            }
//            playable.RebuildGraph();
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
//            track.muted = false;
        }
    }
}