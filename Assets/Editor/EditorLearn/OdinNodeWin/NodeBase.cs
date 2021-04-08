using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorLearn.OdinNodeWin
{
    public class NodeBase
    {
        public UnityEngine.Rect rect = new Rect(0,0, 200, 200);
        public string name = "NodeBase";
        public string type = "基类";
    }
    
    public class NodeMapShow:Attribute
    {  
    }
    
    public class NodeMapShowDrawer:OdinAttributeDrawer<NodeMapShow, NodeBase>
    {
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            NodeBase data = this.ValueEntry.SmartValue;
            GUI.Box(data.rect, data.name, GUIStyle.none);
            var rect = data.rect;
            data.rect = EditorGUI.RectField(new Rect(rect.x, rect.y + 20, rect.width, 50), data.rect);
            
//            Rect rect = EditorGUILayout.GetControlRect();
//            if (label != null)
//            {
//                label.text = label.text + " Type2";
//                rect = EditorGUI.PrefixLabel(rect, label);
//            }
            this.ValueEntry.SmartValue = data;
        }  
    }
}