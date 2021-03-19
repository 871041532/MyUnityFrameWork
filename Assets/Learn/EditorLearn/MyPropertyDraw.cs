using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorLearn
{
    public enum Sex
    {
        famale, // 雌性
        male, // 雄性
    }

    [System.Serializable]
    public class Persion
    {
        public string name;
        public Sex sex;
        public int age = 18;
        public string description;
    }

    public class MyPropertyDraw : MonoBehaviour
    {
        public Persion persion = new Persion();
        public int id;
        public string playerName;
        public string backStory;
        public float health = 50f;
        public float damage;

        public float weaponDamage1, weaponDamage2;

        public string shoeName;
        public int shoeSize;
        public string shoeType;


        // Range()属性用于将一个值指定在一定的范围内，并在Inspector面板中为其添加滑块；
        // header()属性用于添加属性的标题
        [Header("年龄")] [Range(-2, 2)] public int age;

        //Multiline()属性用于给 string 类型添加多行输入  
        [Multiline(3)] public string name1;

        // Tooptip属性用于在 Inspector 面板中，当鼠标停留在设置了Tooltip的属性添加指定的提示；
        // Space用于为在 Inspector 面板两属性之间添加指定的距离
        [Space(20)] [Tooltip("用于设置性别!")] public string sex;

        // SerializeField 序列化域(强制序列化)：可以将私有变量序列化，将U3D的内建变量序列化等。
        [SerializeField, Range(-2, 2)] private int age2;

        [ContextMenuItem("右击name属性点击调用OutputInfo1", "OutputInfo1")]
        public string name2 = "MyPropertyDraw";

        // ContextMenu 允许添加一个命令到该组件上，你可以通过右键或者点击设置图标来调用到它（一般用于函数），且是在非运行状态下执行该函数
        [ContextMenu("OutputInfo")]
        void OutputInfo()
        {
            Debug.Log("OutputInfo");
        }

        void OutputInfo1()
        {
            Debug.Log("OutputInfo1");
        }
    }
}