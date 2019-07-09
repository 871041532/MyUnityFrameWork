using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class ABMgr:IMgr
 {
    public AssetBundle testAb;

    public override void Awake() {
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    // 自定义Altas加载
    void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
    {
        Debug.Log("加载Altas：" + tag);
        string path = Path.Combine("Assets/UI/res", tag + "_sd.spriteatlas");
#if UNITY_EDITOR
        SpriteAtlas sa = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        action(sa);
#endif
    }

    public override void OnDestroy() { }
}

