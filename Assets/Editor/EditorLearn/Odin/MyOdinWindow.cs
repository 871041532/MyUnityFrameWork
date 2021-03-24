using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EditorLearn
{
    // Odlin窗口，其实也继承自EditorWindow
    public class MyOdinWindow:OdinEditorWindow
    {
        [PropertyOrder(-10)]
        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void Button1() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void Button2() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large)]
        public void Button3() { }
        
        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void Button4() { }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
        public void Button5() { }

        
        [TableList]
        public List<SomeType> SomeTableData = new List<SomeType>(){};

        // 绘制自定义属性之前绘制原生Editor
        protected override void OnBeginDrawEditors()
        {
        }
        
        // 绘制自定义属性之后绘制原生Editor
        protected override void OnEndDrawEditors()
        {
        }
    }
    
    public class SomeType
    {
        [TableColumnWidth(50)]
        public bool Toggle;

        [AssetsOnly]
        public GameObject SomePrefab;

        public string Message;

        [TableColumnWidth(160)]
        [HorizontalGroup("Actions")]
        public void Test1() { }

        [HorizontalGroup("Actions")]
        public void Test2() { }
    }
}