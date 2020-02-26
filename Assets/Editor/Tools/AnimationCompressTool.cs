using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AnimationCompress
{
    static readonly HashSet<string> _fixedPositionAndScaleNode = new HashSet<string>
    {
        "leg_l_thigh",
        "leg_l_knee",
    };

    static readonly HashSet<string> _fixedScaleNode = new HashSet<string>
    {
        "hips",
        "leg_l_thigh_Middle",
    };

    static readonly HashSet<string> _fixedPositionNode = new HashSet<string>
    {
        "head_neck_Middle",
    };


    [MenuItem("Tools/Animation压缩", false, 1000)]
    public static void Compress()
    {
        if (true)
        {
            Compress(true);
        }
        else
        {
            var anims = AssetDatabase.FindAssets("t:animationClip", new string[] { "Assets/Build/Art/Character/Animations" });

            for (int i = 0; i < anims.Length; i++)
            {
                anims[i] = AssetDatabase.GUIDToAssetPath(anims[i]);

                if (anims[i].EndsWith(".anim", System.StringComparison.OrdinalIgnoreCase))
                {
                    Compress(anims[i], Mathf.InverseLerp(0, anims.Length - 1, i));
                }
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("动画减帧", "动画减帧完成", "确定");
        }

    }

    static void Compress(string path ,float t)
    {

    }

    [MenuItem("Tools/Animation减帧", false, 1001)]
    public static void ReduceCurve()
    {
        Compress(false);
    }

    static void Compress(bool reducePrecision)
    {
        var obj = Selection.activeObject;
        var folderPath = string.Empty;

        if (obj is AnimationClip)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            EditorUtility.DisplayProgressBar("动画压缩", path, 0);

            var src = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            var target = Object.Instantiate(src);
            target.ClearCurves();

            Compress(src, target, reducePrecision);

            //AssetDatabase.CreateAsset(target, path);
            //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            folderPath = path.Substring(0, path.LastIndexOf('.'));
            AssetDatabase.CreateAsset(target, folderPath + "_op.anim");
            AssetDatabase.ImportAsset(folderPath, ImportAssetOptions.ForceUpdate);

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("动画压缩", "动画压缩完成", "确定");
        }
    }

    private static void Compress(AnimationClip src, AnimationClip target, bool reducePrecision)
    {
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(src);
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(src);

        AnimationUtility.SetAnimationClipSettings(target, settings);

        float curIdx = 0;
        foreach (var b in bindings)
        {
            curIdx += 1f;
            EditorUtility.DisplayProgressBar("动画压缩", b.path, curIdx / bindings.Length);

            bool isPositionCurve = b.propertyName.IndexOf("Position") > -1;
            bool isScaleCurve = b.propertyName.IndexOf("Scale") > -1;

            bool isFixedPositionAndScaleNode = false;
            bool isFixedScaleNode = false;
            bool isFixedPositionNode = false;
            foreach (var e in _fixedPositionAndScaleNode)
            {
                if (b.path.EndsWith(e))
                {
                    isFixedPositionAndScaleNode = true;
                    break;
                }
            }

            if (!isFixedPositionAndScaleNode)
            {
                foreach (var e in _fixedScaleNode)
                {
                    if (b.path.EndsWith(e))
                    {
                        isFixedScaleNode = true;
                        break;
                    }
                }

                if (!isFixedScaleNode)
                {
                    foreach (var e in _fixedPositionNode)
                    {
                        if (b.path.EndsWith(e))
                        {
                            isFixedPositionNode = true;
                            break;
                        }
                    }

                }
            }

            if (isFixedPositionAndScaleNode)
            {
                if (isPositionCurve || isScaleCurve)
                {
                    continue;
                }
            }
            else if (isFixedScaleNode)
            {
                if (isScaleCurve)
                {
                    continue;
                }
            }
            else if (isFixedPositionNode)
            {
                if (isPositionCurve)
                {
                    continue;
                }
            }

            var c = AnimationUtility.GetEditorCurve(src, b);

            if(reducePrecision)
            {
                ReduceFloatPrecision(c);
            }
           
            AnimationUtility.SetEditorCurve(target, b, c);
        }
    }

    private const int Precision = 1000000;

    private static void ReduceFloatPrecision(AnimationCurve curve)
    {
        if (curve == null || curve.keys == null)
        {
            return;
        }

        Keyframe[] keys = curve.keys;
        for (int k = 0; k < keys.Length; k++)
        {
            keys[k].time = Mathf.Round(keys[k].time * Precision) / Precision;
            keys[k].value = Mathf.Round(keys[k].value * Precision) / Precision;
            keys[k].inTangent = Mathf.Round(keys[k].inTangent * Precision) / Precision;
            keys[k].outTangent = Mathf.Round(keys[k].outTangent * Precision) / Precision;
        }

        //过滤位移值没有变化的帧动画
        //因为帧信息有初始位置，所有要保留头尾两帧，如果全部删除会出现初始位置为默认值的问题
        if (IsFilterKeyFrame(ref keys))
        {
            var newKeys = new Keyframe[2];
            newKeys[0] = keys[0];
            newKeys[1] = keys[keys.Length - 1];
            keys = newKeys;
        }

        curve.keys = keys;
    }

    private static bool IsFilterKeyFrame(ref Keyframe[] keys)
    {
        for (var i = 0; i < keys.Length - 1; i++)
        {
            if (Mathf.Abs(keys[i].value - keys[i + 1].value) > 0 ||
              Mathf.Abs(keys[i].outTangent - keys[i + 1].outTangent) > 0
              || Mathf.Abs(keys[i].inTangent - keys[i + 1].inTangent) > 0)
            {
                return false;
            }
        }
        return true;
    }

}
