using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorLearn
{
    // 普通的Odlin窗口，其实也继承自EditorWindow
    public class MyOdinWindow:OdinEditorWindow
    {
        [PropertyOrder(-10)]
        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Large)]
        public void Button1() { }

        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Large)]
        public void Button2() { }

        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Large)]
        public void Button3() { }
        
        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void Button4() { }

        [HorizontalGroup("Button")]
        [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
        public void Button5() { }

       
        [EnumToggleButtons, BoxGroup("Settings")]
        public ScaleMode ScaleMode;

        [FolderPath(RequireExistingPath = true), BoxGroup("Settings")]
        public string OutputPath;

        [HorizontalGroup("SelectTexture", 0.5f)]
        public List<Texture> InputTextures = new List<Texture>();
        

        [HorizontalGroup("SelectTexture", 0.5f), InlineEditor(InlineEditorModes.LargePreview)]
        public Texture Preview;

        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void PerformSomeAction()
        {

        }
        
       
        
        protected override void Initialize()
        {
            this.WindowPadding = Vector4.one;
        }
       
        
        // 绘制自定义属性之前绘制原生Editor
        protected override void OnBeginDrawEditors()
        {
        }
        
        // 绘制自定义属性之后绘制原生Editor
        protected override void OnEndDrawEditors()
        {
        }
    }
    
}