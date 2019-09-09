using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;

class CheckRecycleReference
{
    private static List<List<string>> m_RecycleTracks;

//    [MenuItem("Assets/Build/CheckRecycleReference", priority = 2)]
    public static void CheckRecycleAB()
    {
        Debug.Log("开始进行循环引用检测...");
        m_RecycleTracks = new List<List<string>>();
        string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var abName in allBundleNames)
        {
            iterAB(abName, new List<string>(){abName});
        }

        if (m_RecycleTracks.Count >0)
        {
            Debug.LogError($"AB包存在循环引用：{m_RecycleTracks.Count}");
            foreach (var l in m_RecycleTracks)
            {
                string strs = "";
                foreach (var item in l)
                {
                    strs += $" -> {item} " ;
                }
                Debug.LogError(strs);
            }
            throw new Exception();
        }
        else
        {
            Debug.Log("检测完毕没有循环引用！");
        }
    }

    static void iterAB(string abName, List<string> track)
    {
        string[] dependBundleNames = AssetDatabase.GetAssetBundleDependencies(abName, false);
        if (dependBundleNames.Length > 0)
        {
            foreach (var dependName in dependBundleNames)
            {
                var newTrack = new List<string>(track);
                if (track.Contains(dependName))
                {
                    newTrack.Add(dependName);
                    m_RecycleTracks.Add(newTrack);
                }
                else
                {
                    newTrack.Add(dependName);
                    iterAB(dependName, newTrack);
                }
            }
        }
    }
}