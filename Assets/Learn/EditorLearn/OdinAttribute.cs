#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace EditorLearn
{
    [CreateAssetMenu(fileName = "ScrptableClass", menuName = "CreateScrptableClass", order = 0)]
    [InlineEditor]
    public class ScrptableClass : ScriptableObject
    {
        public Dictionary<string, string> dict;
        [LabelText("名字")] public string name;
        [LabelText("年龄")] public int age;
    }

    // Odin属性demo
    [ExecuteInEditMode]
    public class OdinAttribute : MonoBehaviour
    {
        /**********************************自定义属性****************************************/
        public PlayableAsset asseet;
        public PlayableGraph graph;
        [LabelText("是否显示自定义属性")] 
        [OnValueChanged("@#(health).State.Visible = $value")]
        [OnValueChanged("@#(myStruct1).State.Visible = $value")]
        [OnValueChanged("@#(myStruct2).State.Visible = $value")]
        [OnValueChanged("@#(showCustomMessage).State.Visible = $value")]
        [OnValueChanged("@#(HaveLabelName).State.Visible = $value")]
        [OnValueChanged("@#(NoLabelName).State.Visible = $value")]
        [OnValueChanged("@#(CustomDrawerArray).State.Visible = $value")]
        public bool isShowCustomeValue = true;
        
        // 可以复制的消息窗口
        [DetailedInfoBox("简介消息", "可以复制的消息窗口", InfoMessageType = InfoMessageType.Info, VisibleIf = "isShowDetailInfoBox"), LabelText("值不为空显示消息")]
        public string showCustomMessage = "1";
        public bool isShowDetailInfoBox()
        {
            return showCustomMessage != "";
        }

        // 自定义字段绘制函数：含label的字段
        [CustomValueDrawer("HaveLabelNameFunc")]
        public string HaveLabelName = "含label的字段";
        public string HaveLabelNameFunc(string tempName, GUIContent label)
        {
            return EditorGUILayout.TextField(label, tempName);
        }
        
        // 自定义字段绘制函数：不含label的字段
        [CustomValueDrawer("NoLabelNameFunc")]
        public string NoLabelName = "不含label的字段";
        public string NoLabelNameFunc(string tempName, GUIContent label)
        {
            return EditorGUILayout.TextField(tempName);
        }
        
        // 自定义绘制：给Slider加进度条
        [CustomValueDrawer("MyCustomDrawerArrayFunc"), LabelText("自定义Slider数组")]
        public float[] CustomDrawerArray = new float[]{1f, 2f, 3f};
        private float MyCustomDrawerArrayFunc(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(value, 0, 5);
        }
        
        // 血量进度条
        [HealthBarAttribute(20), LabelText("自定义float进度条")]
        public float health = 7.5f;

        // 绘制自定义类1
        [LabelText("自定义Struct1")] public MyStruct myStruct1;

        // 绘制自定义类2
        [MyStructAttribute, LabelText("自定义Struct1")]
        public MyStruct myStruct2;

        // 自定义带颜色的FoldoutGroup标签 
        [ColoredFoldoutGroupAttribute("Green", 0, 1, 0)]
        [ShowIf("@this.isShowCustomeValue == true")]
        public string Name;

        [ColoredFoldoutGroupAttribute("Green")]
        [ShowIf("@this.isShowCustomeValue == true")]
        public int AA;

        [ColoredFoldoutGroupAttribute("Blue", 0, 0, 1)]
        [ShowIf("@this.isShowCustomeValue == true")]
        public float BB;

        [ColoredFoldoutGroupAttribute("Blue", 0, 0, 1)] [ProgressBar(0, 1), LabelText("进度条")]
        [ShowIf("@this.isShowCustomeValue == true")]
        public float progress1;

        /**********************************普通属性描述*********************************************/
        // 多行文本编辑框
        [MultiLineProperty(5), LabelText("多行string")]
        public string odinMultiString = "多行文本内容...";
        
        // 下拉从一个list中多选一1
        [ValueDropdown("drapDownList1", AppendNextDrawer = true, DisableGUIInAppendedDrawer = true), LabelText("下拉从一个list中多选一1")]
        public int dropDownValue1 = 2;
        private List<int> drapDownList1 = new List<int>()
        {
              1,2,3
        };
        // 下拉从一个list中多选一2
        [ValueDropdown("drapDownList2", SortDropdownItems =  true, AppendNextDrawer = true, DisableGUIInAppendedDrawer = true), LabelText("下拉从一个list中多选一2")]
        public int dropDownValue2 = 2;
        private List<ValueDropdownItem<int>> drapDownList2 = new ValueDropdownList<int>()
        {
            {"one", 1},
            {"two", 2},
        };
        // 进度条
        [ProgressBar(0, 1), LabelText("进度条")] public float progress2;

        // 文件路径属性
        [FilePath(Extensions = ".unity"), LabelText("文件路径属性")]
        public string ScenePath;

        // 字典属性
        [ShowInInspector] [LabelText("字典字典")] public Dictionary<string, string> m_dict;

        // 按钮属性
        [Button("按钮属性", ButtonSizes.Large)]
        public void SayHello()
        {
            Debug.Log("Hello button!");
        }

        // 普通枚举
        [LabelText("普通枚举")] 
        public ExampleEnum exampleEnum1;

        // 左右滑动枚举
        [EnumPaging, LabelText("左右滑动枚举")] public ExampleEnum exampleEnum2;

        // 按钮单选枚举
        [EnumToggleButtons, LabelText("按钮单选枚举")]
        public ExampleEnum exampleEnum3;

        // 隐藏public
        [HideInInspector, LabelText("隐藏public")]
        public int NormallyVisible;

        // 显示private
        [ShowInInspector, LabelText("显示private")]
        private bool NormallyHidden;

        // 显示getset
        [ShowInInspector, LabelText("显示GetSet")]
        public ScriptableObject Property { get; set; }

        [LabelText("序列化对象")] public ScriptableObject Property2;

        // 不能为空的Project中的prefab
        [PreviewField, Required, AssetsOnly, LabelText("Project中的对象")]
        public GameObject AssetsOnlyPrefab;

        // 可以为空的Scene中的GameObject
        [PreviewField, SceneObjectsOnly, LabelText("Scene中的对象")]
        public GameObject SceneOnlyPrefab;
        
        
        // 左边出现选择块，点击选择包括自己在内的节点
        [ChildGameObjectsOnly, LabelText("子节点对象")]
        public GameObject ChildGameObject;

        // 横纵布局的属性，路径决定了布局
        [HorizontalGroup("Split", Width = 50), HideLabel, PreviewField(50)]
        public Texture2D Icon;

        [VerticalGroup("Split/Properties")] public string MinionName;
        [VerticalGroup("Split/Properties")] public float Health;
        [VerticalGroup("Split/Properties")] public float Damage;

        // 将字符串直接显示到名字上
        [LabelText("@IAmLabel")] public string IAmLabel = "指定字符串属性显示";

        /************************************带布局的属性**********************************************/
        // ResponsiveButtonGroup 将多个按钮组织在一个组中，窗口缩小时会自动换行
        [ResponsiveButtonGroup]
        public void ResponsiveGroup1(){}
        [ResponsiveButtonGroup]
        public void ResponsiveGroup2(){}
        [ResponsiveButtonGroup]
        public void ResponsiveGroup3(){}
        
        // ButtonGroup 将多个按钮组织在一个整齐的水平组中（窗口缩小时不会自动换行）。
        [ButtonGroup]
        public void ButtonGroup1()
        {
            
        }
        [ButtonGroup]
        public void ButtonGroup2()
        {
            
        }
        [ButtonGroup]
        public void ButtonGroup3()
        {
            
        }
        // TitleGroup标题布局
       
        public bool Toggle; 
        [TitleGroup("TitleGroup标题布局")]
        public int titleGroupInt1;
        [TitleGroup("TitleGroup标题布局", "副标题、副标题、副标题")]
        public int titleGroupInt2;
        
        // tab页签布局的属性，相同tab名字会被放到一个页面上
        [TabGroup("Tab页1")] public int FirstTab;

        [ShowInInspector, TabGroup("Tab页1")] public int SecondTab { get; set; }

        [TabGroup("Tab页2")] public float FloatValue;

        [TabGroup("Tab页2"), Button]
        public void Button()
        {
        }
        
        // 将按钮添加到属性的末尾
        [InlineButton("InlineButton"), LabelText("属性旁边有按钮")]
        public int InlineButtonField;

        private void InlineButton()
        {
            Debug.Log("点击了InlineButton");
        }

        
        // ToggleGroup用于任何字段，并创建一组可切换的选项。使用此选项可以创建可以启用或禁用的选项。
        [ToggleGroup("MyToggleGroup", "折叠区域并设置可否编辑")]
        public bool MyToggleGroup;

        [ToggleGroup("MyToggleGroup")] public float ToggleGroupA;
        [ToggleGroup("MyToggleGroup")] public string ToggleGroupB;

        // 带折叠标签的，组合按钮布局，相同路径的名字会被放到一起。相同目录层级会受到一个标签安排
        [Button(ButtonSizes.Large)]
        [FoldoutGroup("折叠加组合布局")] //PropertyOrder()
        [HorizontalGroup("折叠加组合布局/Horizontal", Width = 60)]
        [BoxGroup("折叠加组合布局/Horizontal/第一页")]
        public void Button1()
        {
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("折叠加组合布局/Horizontal/第二页")]
        public void Button2()
        {
        }

        [Button]
        [BoxGroup("折叠加组合布局/Horizontal/第三页")]
        public void Accept()
        {
        }

        [Button]
        [BoxGroup("折叠加组合布局/Horizontal/第三页")]
        public void Cancel()
        {
            RigidbodyPrefab = new GameObject();
        }

        /******************************带触发和校验行为的属性***************************************/
        // 右键属性弹出菜单发出指定函数，配套的DisableContextMenu可以关闭右键菜单
        [InfoBox("右键属性可在菜单中发出指定的函数")]
        [CustomContextMenu("自定义菜单/sayHello", "SayHelloFunction"), LabelText("自定义右键菜单")]
        public int customContextMenu;
        private void SayHelloFunction()
        {
            Debug.Log("hello，hello");
        }
        
        
        // 一个可以触发创建删除函数的GUID列表
        [LabelText("会触发创建删除函数的列表"),
         ListDrawerSettings(CustomAddFunction = "CreateNewGUID", CustomRemoveIndexFunction = "RemoveGUID")]
        public List<string> GuidList = new List<string>() {"hello world!"};

        private string CreateNewGUID()
        {
            Debug.Log("创建一个GUID！");
            return Guid.NewGuid().ToString();
        }

        private void RemoveGUID(int index)
        {
            Debug.Log($"删除一个GUID, index={index}");
            this.GuidList.RemoveAt(index);
        }

        // 带正确性校验的属性，当值改变时会调用对应的check方法
        [ValidateInput("IsValid")] [LabelText("一个只能大于0的数字")]
        public int GreaterThanZero;

        private bool IsValid(int value)
        {
            return value > 0;
        }

        // 当本属性被改变时，会调用对应的方法来更新其他属性
        [LabelText("属性被改变CallFunc")] [OnValueChanged("UpdateRigidbodyReference")]
        public GameObject RigidbodyPrefab;

        private Rigidbody prefabRigidbody;

        private void UpdateRigidbodyReference()
        {
            Debug.Log("RigidbodyPrefab修改了，开始更新prefabRigidbody！");
            if (this.RigidbodyPrefab != null)
            {
                this.prefabRigidbody = this.RigidbodyPrefab.GetComponent<Rigidbody>();
            }
            else
            {
                this.prefabRigidbody = null;
            }
        }

        // 当本属性被改变时，询问对应函数，返回为True时显示对应消息
        [InfoBox("当 MyInt 是偶数时才显示这个消息！", "IsEven"), LabelText("MyInt属性")]
        public int MyInt;

        private bool IsEven()
        {
            return this.MyInt % 2 == 0;
        }

        /*********************************状态表达式******************************************/
        /*
         *  状态表达式有 Visible  Enabled  Expanded 三个属性
         */

        // 当IsShowMyStr为true时才展示myStr字段
        [LabelText("展示MyStr的InfoBox？")] public bool IsShowMyStr = true;

        // 直接把字符串显示在InfoBox上
        [InfoBox("@myStr")]
        // 直接把当前时间显示在InfoBox上
        [InfoBox(@"@""当前时间: "" + DateTime.Now.ToString(""HH:mm:ss"")")]
        // 在某种情况下才显示本字段
        [ShowIf("@this.IsShowMyStr == true")]
        public string myStr = "myStr默认值";

        // 直接获取本字段的值，显示在LabelText上
        [LabelText("@$value")] public string myStr2 = "Value即是LabelText";


        // 列表的折叠状态由一个bool字段决定（仍然能手动折叠展开，和上面ShowIf的区别是，这个放在控制字段上，而不是被控制字段上）
        [OnValueChanged("@#(someList).State.Expanded = $value"), LabelText("是否展开列表")]
        public bool expandList = false;

        [LabelText("被折叠列表")] public List<int> someList = new List<int>() {1, 2, 3};

        // 当枚举值包函位域时，展示某个值（使用OnStateUpdate不能手动折叠展开）
        [OnStateUpdate("@#(exampleList).State.Expanded = $value.HasFlag(ExampleEnum.UseStringList)")]
        [LabelText("折叠列表的枚举")]
        public ExampleEnum exampleEnum;

        [LabelText("被枚举折叠的列表")] public List<string> exampleList;

        [Flags]
        public enum ExampleEnum
        {
            None,
            UseStringList = 1 << 0,
            Other = 1 << 1,
        }


        // 拖拽进度条，实现tab自动被选择状态的变化 （Tab的选择只能被进度条控制）   
        [OnStateUpdate("@#(#Tabs1).State.Set<int>(\"CurrentTabIndex\", $value - 1)")]
        [PropertyRange(1, "@#(#Tabs1).State.Get<int>(\"TabCount\")")]
        [LabelText("进度条拖拽改变Tab选择")]
        public int selectedTab = 1;

        [TabGroup("Tabs1", "Tab 1")] public string exampleString1;
        [TabGroup("Tabs1", "Tab 2")] public string exampleString2;
        [TabGroup("Tabs1", "Tab 3")] public string exampleString3;

        private void Awake()
        {
        }

        // *********************Property Tree属性树绘制************************************** //
        // 单个属性
        private MyStruct treeProperty1 = new MyStruct();
        private PropertyTree objectTree1;

        private void DrawSingleTree()
        {
            if (this.objectTree1 == null)
            {
                this.objectTree1 = PropertyTree.Create(this.treeProperty1);
            }

            this.objectTree1.Draw(false);
        }

        // 多个属性
        private Vector2[] treeProperty2 = new Vector2[] {Vector2.up, Vector2.down};
        private PropertyTree objectTree2;

        private void DrawMultipleTree()
        {
            if (this.objectTree2 == null)
            {
                objectTree2 = PropertyTree.Create(this.treeProperty2);
            }
            this.objectTree2.Draw(false);
        }


        private void OnGUI()
        {
            this.DrawSingleTree();
            this.DrawMultipleTree();
        }
    }
}
#endif