using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EditorLearn.OdinNodeWin
{
    public class OdinNodeWin:OdinEditorWindow
    {
        [NodeMapShow, HideLabel]
        [CustomContextMenu("sayHello", "SayHelloFunction"), LabelText("自定义右键菜单")]
        public NodeBase node1 = new NodeBase();

        private void SayHelloFunction()
        {
            Debug.Log("Hello world");
        }
        
    }
}