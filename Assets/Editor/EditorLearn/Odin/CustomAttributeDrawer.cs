using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorLearn
{  
    // 绘制float属性类（只能在Editor下面使用）
    public class HealthBarAttributeDrawer:OdinAttributeDrawer<HealthBarAttribute, float>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            /********************用odin的方式扩展float**********************/ 
            // label.text = "用Odin扩展float";// In this case, we don't need the label for anything, so we will just pass it to the next drawer.
            this.CallNextDrawer(label);
            // Get a rect to draw the health-bar on.
            Rect rect = EditorGUILayout.GetControlRect();
            // Draw the health bar using the rect.
            float width = Mathf.Clamp01(this.ValueEntry.SmartValue / this.Attribute.MaxHealth);
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0, 0, 0, 0), false);
            SirenixEditorGUI.DrawSolidRect(rect.SetWidth(rect.width * width), Color.red, false);
            SirenixEditorGUI.DrawBorders(rect, 1);

            
            /**************************用编辑器原生扩展float*********************************/
            //使用滑块绘制 Player 生命值
            this.ValueEntry.SmartValue = EditorGUILayout.Slider("用原生扩展float", this.ValueEntry.SmartValue, 0, this.Attribute.MaxHealth);
            //根据生命值设置生命条的背景颜色
            if (this.ValueEntry.SmartValue < 4)
            {
                GUI.color = Color.red;
            }
            else if (this.ValueEntry.SmartValue > 16)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.gray;
            }
            //指定生命值的宽高
            Rect progressRect = GUILayoutUtility.GetRect(50, 50);
            //绘制生命条
            EditorGUI.ProgressBar(progressRect, this.ValueEntry.SmartValue / this.Attribute.MaxHealth, "Health");
            //用此处理，以防上面的颜色变化会影响到下面的颜色变化
            GUI.color = Color.white;
        }
    }


    // 绘制普通自定义类1(方式一：直接扩展类，作为这个类的默认展示方式)
    public class MyStructDrawer1 : OdinValueDrawer<MyStruct>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            MyStruct value = this.ValueEntry.SmartValue;
            GUIHelper.PushLabelWidth(20);
            value.X = EditorGUI.Slider(rect.AlignLeft(rect.width * 0.5f), "X", value.X, 0, 100);
            value.Y = EditorGUI.Slider(rect.AlignRight(rect.width * 0.5f), "Y", value.Y, 0, 100);
            GUIHelper.PopLabelWidth();
            this.ValueEntry.SmartValue = value;
        }
    }

    // 绘制普通自定义类2（扩展类的Attribute，当MyStruct类对象指定这个标签时，会覆盖默认的显示）
    public class MyStructDrawer2:OdinAttributeDrawer<MyStructAttribute, MyStruct>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            if (label != null)
            {
                label.text = label.text + " Type2";
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            MyStruct value = this.ValueEntry.SmartValue;
            GUIHelper.PushLabelWidth(20);
            value.X = EditorGUI.Slider(rect.AlignLeft(rect.width * 0.5f), "横", value.X, 0, 100);
            value.Y = EditorGUI.Slider(rect.AlignRight(rect.width * 0.5f), "纵", value.Y, 0, 100);
            GUIHelper.PopLabelWidth();
            this.ValueEntry.SmartValue = value;
        }  
    }
    
    // 绘制自定义Group布局的Drawer，效果是带颜色的foldoutGroup
    public class ColoredFoldoutGroupAttributeDrawer:OdinGroupDrawer<ColoredFoldoutGroupAttribute>
    {
        private LocalPersistentContext<bool> isExpanded;

        protected override void Initialize()
        {
            this.isExpanded = this.GetPersistentValue<bool>("oloredFoldoutGroupAttributeDrawer.isExpanded", GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUIHelper.PushColor(new Color(this.Attribute.r, this.Attribute.g, this.Attribute.b, this.Attribute.a));
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            GUIHelper.PopColor(); 
            
            this.isExpanded.Value = SirenixEditorGUI.Foldout(this.isExpanded.Value, label);
            SirenixEditorGUI.EndBoxHeader();

            if (SirenixEditorGUI.BeginFadeGroup(this, this.isExpanded.Value))
            {
                for (int i = 0; i < this.Property.Children.Count; i++)
                {
                    this.Property.Children[i].Draw();
                }
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}