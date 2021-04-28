using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorLearn
{
    enum EnumOption
    {
        CUBE = 0,
        SPHERE = 1,
        PLANE = 2
    }

    // 自定义窗口
    public class MyWindow : EditorWindow
    {
        string myString = "Hello World, Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
        public Vector2 scrollPos = Vector2.zero;
        public Vector2 scrollPosBig = Vector2.zero;
        private bool foldOut = true;
        private GameObject gameObjectField = null;
        private Material materialField = null;
        private string password;
        private int optionIndex;
        private EnumOption optionEnum = EnumOption.CUBE;
        private int toolBarSelect;
        private string filePath = "";
        private int gridSelection;
        private string rectString;
        private GameObject dragObject;
        private AnimationCurve curve;

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
            if (Selection.objects.Length > 0)
            {
                Debug.Log(Selection.objects[0]);
            }
        }

        private void OnProjectChange()
        {
            Debug.Log("MyWindow 项目状态发生更改"); // 比如导入资源等操作
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

            // ToolBar工具栏，横向选择
            string[] toolBarStrs = {"Selection1", "Selection2", "Selection3"};
            toolBarSelect = GUILayout.Toolbar(toolBarSelect, toolBarStrs, GUILayout.Height(25));

            // 非下拉选择网格，可以给横向或者纵向
            string[] gridStrs = {"Message1", "Message2", "Message3", "Message4"};
            gridSelection = GUILayout.SelectionGrid(gridSelection, gridStrs, 2);

            // 下拉选项菜单
            string[] options = {"Popup选项一", "Popup选项二", "Popup选项三"};
            var oldValue = optionIndex;
            optionIndex = EditorGUILayout.Popup(optionIndex, options);
            if (optionIndex != oldValue)
            {
                Debug.Log("选项发生变化：" + optionIndex);
            }

            // 整数下拉选项
            string[] names = {"number1", "number2", "number4"};
            int[] sizes = {1, 2, 4};
            int selectSize = 4;
            EditorGUILayout.IntPopup("整数下拉: ", selectSize, names, sizes);

            // 枚举选项菜单
            optionEnum = (EnumOption) EditorGUILayout.EnumPopup("枚举选项:", optionEnum);
            EditorGUILayout.PrefixLabel("PreFixLabel");
            // 带箭头的折叠项
            foldOut = EditorGUILayout.Foldout(foldOut, "Foldout");
            if (foldOut)
            {
                // 标签字段
                GUILayout.Label("  隐藏项", EditorStyles.boldLabel);
            }

           

            // 标签字段
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.fontSize = 20;
            EditorGUILayout.LabelField("Base Settings", fontStyle, GUILayout.MinHeight(25));
            
            // 曲线属性
            curve = EditorGUILayout.CurveField("曲线属性", curve);
            
            // 整数字段
            EditorGUILayout.IntField("整数字段", 1);

            // 浮点数字段
            EditorGUILayout.FloatField("浮点数字段", 1.25f);

            // 二维向量字段
            EditorGUILayout.Vector2Field("二维向量字段", Vector2.left);

            // 颜色字段
            EditorGUILayout.ColorField("颜色字段", Color.yellow);

            // GameObject字段
            gameObjectField =
                EditorGUILayout.ObjectField("gameObjectField", gameObjectField, typeof(GameObject),
                    false) as GameObject;
            if (gameObjectField != null)
            {
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectField);
                EditorGUILayout.LabelField("prefab: " + prefabPath);
            }

            // tag拾取文本，给gameObject设置tag
            EditorGUILayout.TagField("GameObjectTag:", "Untagged");

            // layer拾取文本，给gameObject设置layer
            EditorGUILayout.LayerField("GameObjectLayer:", 0);

            // materialField字段
            materialField =
                EditorGUILayout.ObjectField("materialField", materialField, typeof(Material), false) as Material;

            // 文本输入框
            myString = EditorGUILayout.TextField("账号：", myString);

            // 密码输入框
            password = EditorGUILayout.PasswordField("Password：", password);

            // 文本输入区域
            EditorGUILayout.PrefixLabel("文本输入区域");
            myString = EditorGUILayout.TextArea(myString);

            // 禁用区域：当groupEnabled为true时才能修改group范围内的值
            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            // 单选框（如果没有myBool = 则不能修改）
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            // 进度条
            myFloat = EditorGUILayout.Slider("Slider", myFloat, 0, 1);
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
            if (EditorWindow.focusedWindow != null)
            {
                EditorGUILayout.LabelField("键盘焦点window：" + EditorWindow.focusedWindow.ToString());
                EditorGUILayout.LabelField("光标下的window:" + EditorWindow.focusedWindow.ToString());
            }

            // 添加滚动区域（此处需要加scrollPos = ，才能改变值）
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(100), GUILayout.MaxHeight(220));
            // 开始窗口里的子窗口
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
            }

            // 保存路径文件夹选取面板
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存文件夹路径", GUILayout.ExpandWidth(false)))
            {
                filePath = EditorUtility.SaveFolderPanel("Path to Save Images", filePath, Application.dataPath);
            }

            EditorGUILayout.LabelField("当前路径：" + filePath);
            EditorGUILayout.EndHorizontal();

            // 保存路径文件选择面板
            if (GUILayout.Button("保存文件路径"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Save texture as PNG",
                    "",
                    "123.png",
                    "png");
                Debug.Log("图片路径：" + path);
            }

            // 文件拖拽控制区域demo
            EditorGUILayout.LabelField("文件拖拽框: ");
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(30));
            rectString = EditorGUI.TextField(rect, rectString);
            EditorGUILayout.ObjectField("拖拽的GameObject", dragObject, typeof(GameObject),
                false);
            if ((Event.current.type == UnityEngine.EventType.DragExited ||
                 Event.current.type == UnityEngine.EventType.DragUpdated) &&
                rect.Contains(Event.current.mousePosition)) //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic; //改变鼠标的外表  
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    rectString = DragAndDrop.paths[0]; // 拖拽的文件
                }

                if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                {
                    dragObject = DragAndDrop.objectReferences[0] as GameObject; // 拖拽的游戏对象   
                }
            }

            // 打开一个通知栏
            if (GUILayout.Button("打开通知栏"))
            {
                this.ShowNotification(new GUIContent("This is a Notification"));
            }

            // 关闭通知栏
            if (GUILayout.Button("关闭"))
            {
                this.RemoveNotification();
            }

            // 打开一个进度条弹窗
            if (GUILayout.Button("打开进度条弹窗", GUILayout.MinHeight(30)))
            {
                EditorUtility.DisplayProgressBar("正在解压", "解压期间不消耗流量...", 0.56f);
            }

            // 清空进度条
            if (GUILayout.Button("清空进度条弹窗", GUILayout.MinHeight(30)))
            {
                EditorUtility.ClearProgressBar();
            }

            //编辑器下创建gameObject
            if (GUILayout.Button("创建游戏对象"))
            {
                GameObject created = EditorUtility.CreateGameObjectWithHideFlags("新建游戏对象",
                    HideFlags.None);
            }

            // 显示普通对话框
            if (GUILayout.Button("显示普通对话框"))
            {
                if (EditorUtility.DisplayDialog("标题，标题", "内容，内容", "确认", "取消"))
                {
                    Debug.Log("点击了确认！");
                }
            }

            // 显示复杂对话框
            if (GUILayout.Button("显示复杂对话框"))
            {
                var option = EditorUtility.DisplayDialogComplex(
                    "标题",
                    "内容，内容...",
                    "确定",
                    "取消",
                    "其他");
                switch (option)
                {
                    case 0:
                        Debug.Log("点击了【确定】");
                        break;
                    case 1:
                        Debug.Log("点击了【取消】");
                        break;
                    case 2:
                        Debug.Log("点击了【其他】");
                        break;
                    default:
                        Debug.Log("default switch");
                        break;
                }
            }

            // DisplayPopupMenu显示弹出菜单栏（比如Assets窗口或者Inspector窗口）
            if (GUILayout.Button("显示弹出菜单栏"))
            {
                Rect contextRect = new Rect(10, 10, 100, 100);
                EditorUtility.DisplayPopupMenu(contextRect, "Assets/", null);
            }

            // 使项目窗口到前面并焦点它，这个通常在一个菜单项创建并选择一个资源之后被调用。 
            if (GUILayout.Button("Focus Unity工程"))
            {
                EditorUtility.FocusProjectWindow();
            }

            // 右键菜单
            var eventItem = Event.current;
            if (eventItem.type == UnityEngine.EventType.ContextClick)
            {
                var mousePos = eventItem.mousePosition;
                Debug.Log("弹出右键菜单：" + mousePos);
                var menu  = new GenericMenu ();
                menu.AddItem (new GUIContent ("选项1"), false, (userData) => { Debug.Log(userData);}, "点击item1");
                menu.AddItem (new GUIContent ("选项2/1"), false, (userData) => { Debug.Log(userData);}, "点击item2.1");
                menu.AddItem (new GUIContent ("选项2/2"), false, (userData) => { Debug.Log(userData);}, "点击item2.2");
                menu.AddSeparator ("");
                menu.AddItem (new GUIContent ("选项3"), false, (userData) => { Debug.Log(userData);}, "点击item3");
                menu.ShowAsContext();
                eventItem.Use();
            }
            
            
            EditorGUILayout.EndScrollView();
        }
    }
}