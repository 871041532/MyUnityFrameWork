using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorLearn
{
    public static class EditorLearnIndex
    {
        //返回当前屏幕上第一个 T 类型的 EditorWindow。
        // 如果没有，则创建并显示新窗口，然后返回其实例。
        [MenuItem("EditorLearn/MyWindow")]
        public static void ShowMyWindow()
        {
            // 第一个参数为true，不可停靠，为false可停靠
            MyWindow window =  EditorWindow.GetWindow<MyWindow>(false, "我的窗口", true);
        }
        
        // 获取一个带Rect的Window
        [MenuItem("EditorLearn/MyWindowRect")]
        public static void ShowMyWindowWithRect()
        {
            var rect = new Rect(100, 100, 300, 300);
            MyWindow window =  EditorWindow.GetWindowWithRect<MyWindow>(rect, false, "我的窗口Rect");
        }
    }
}