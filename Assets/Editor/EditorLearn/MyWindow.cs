using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace EditorLearn
{
    public class MyWindow : EditorWindow
    {
        string myString = "Hello World, Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
        public Vector2 scrollPos = Vector2.zero;
        public Vector2 scrollPosBig = Vector2.zero;
        private bool foldOut = true;

        private void Awake()
        {
            Debug.Log("MyWindow 创建");
        }

        private void OnDestroy()
        {
            Debug.Log("MyWindow 销毁");
        }

        
        private void OnFocus()
        {
            Debug.Log("MyWindow 获得焦点");
        }

        private void OnLostFocus()
        {
            Debug.Log("MyWindow 失去焦点");
        }

        private void OnSelectionChange()
        {
            Debug.Log("MyWindow 选择发生变化");
        }

        private void OnProjectChange()
        {
            Debug.Log("MyWindow 项目状态发生更改");  // 比如导入资源等操作
        }

        private void OnHierarchyChange()
        {
            Debug.Log("MyWindow Hierachy变化");
        }

        private void OnInspectorUpdate()
        {
//            Debug.Log("MyWindow 每秒10帧调用");
        }

        void OnGUI()
        {
            // 此处为实际窗口代码
            scrollPosBig = EditorGUILayout.BeginScrollView(scrollPosBig);
            
            // 前缀标签，会和下面紧接的项一起被选中：
            EditorGUILayout.PrefixLabel("PreFixLabel");
            // 带箭头的折叠项
            foldOut = EditorGUILayout.Foldout(foldOut, "Foldout");
            if (foldOut)
            {  
                // 标签字段（黑体）
                GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
            }

            // 标签字段(普通字体)
            EditorGUILayout.LabelField("Base Settings"); 
            
            // 整数字段
            EditorGUILayout.IntField("整数字段", 1);
            
            // 浮点数字段
            EditorGUILayout.FloatField("浮点数字段", 1.25f);
            
            // 二维向量字段
            EditorGUILayout.Vector2Field("二维向量字段", Vector2.left);
            
            // 颜色字段
            EditorGUILayout.ColorField("颜色字段", Color.yellow);
            
            // 文本输入框
            myString = EditorGUILayout.TextField("文本输入框", myString);
            
            // 文本输入区域
            EditorGUILayout.PrefixLabel("文本输入区域");
            myString = EditorGUILayout.TextArea(myString);
            
            // 禁用区域：当groupEnabled为true时才能修改group范围内的值
            groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);           
            // 单选框（如果没有myBool = 则不能修改）
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);  
            // 进度条
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, 0, 1);
            //指定生命值的宽高
            Rect progressRect = GUILayoutUtility.GetRect(50, 50);
            // 用滑块绘制生命条
            GUI.color = Color.cyan;
            EditorGUI.ProgressBar(progressRect, myFloat, "生命值");
            GUI.color = Color.white;
            EditorGUILayout.EndToggleGroup();
            
            // 帮助盒子
            EditorGUILayout.HelpBox("伤害太低了吧！！", MessageType.Error);
            EditorGUILayout.HelpBox("伤害有点高啊！！", MessageType.Warning);
            EditorGUILayout.HelpBox("伤害适中！！", MessageType.Info);
            
            // 焦点和光标
            EditorGUILayout.LabelField("键盘焦点window："+EditorWindow.focusedWindow.ToString());
            EditorGUILayout.LabelField("光标下的window:" + EditorWindow.focusedWindow.ToString());
            
            // 添加滚动区域（此处需要加scrollPos = ，才能改变值）
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(100), GUILayout.MaxHeight(220));
            // 开始子窗口
            this.BeginWindows();
            var windowRect = new Rect(0, 0, 100, 100);
            GUILayout.Window(1, windowRect, (int winID) =>
            {
                GUILayout.Button(winID.ToString());
                GUILayout.Button(scrollPos.ToString());
                GUI.DragWindow();
            }, "子窗口1");
            windowRect.y += 100;
            GUILayout.Window(2, windowRect, (int winID) =>
            {
                GUILayout.Button(winID.ToString());
                GUILayout.Button(scrollPos.ToString());
                GUI.DragWindow();
            }, "子窗口2");
            // 子窗口结束
            this.EndWindows();
            EditorGUILayout.LabelField("", GUILayout.Width(100), GUILayout.Height(200));
            // 滚动区域结束
            EditorGUILayout.EndScrollView();   
            
            //空一行
            EditorGUILayout.Space();
            
            //以水平方向绘制
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));
            EditorGUILayout.TextField("小丽");
            EditorGUILayout.LabelField("Size", GUILayout.MaxWidth(50));
            EditorGUILayout.TextField("36D");
            EditorGUILayout.EndHorizontal();
            
            // 发送事件
            if (GUILayout.Button("发送事件", GUILayout.ExpandHeight(true)))
            {
                var win = EditorWindow.GetWindow<MyWindow>();
                win.SendEvent(EditorGUIUtility.CommandEvent("【按钮事件】"));
            }
            // 接收事件
            Event e = Event.current;
            if (e.commandName != "")
            {
                Debug.Log("收到：" + e.commandName); 
                e.commandName = "";
            }
            
            EditorGUILayout.EndScrollView(); 
        }
    }
}