#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode] //让脚本能够在编辑器模式下运行
public class ProceduralTextureGeneration: MonoBehaviour {
    public Material m_material = null;
    private Texture2D m_generatedTexture = null;

    #region Material properties
    [SerializeField,SetProperty("textureWidth")]
    private int m_textureWidth = 512;
    public int textureWidth
    {
        get { return m_textureWidth; }
        set { m_textureWidth = value; _UpdateMaterial(); }
    }

    [SerializeField,SetProperty ("backgroundColor")]
    private Color m_backgroundColor = Color.white;
    public Color backgroundColor
    {
        get { return m_backgroundColor; }
        set { m_backgroundColor = value; _UpdateMaterial(); }
    }

    [SerializeField, SetProperty("circleColor")]
    private Color m_circleColor = Color.yellow;
    public Color circleColor
    {
        get { return m_circleColor; }
        set { m_circleColor = value;_UpdateMaterial(); }
    }

    [SerializeField, SetProperty("blurFactor")]
    private float m_blurFactor = 2.0f;
    public float blurFactor
    {
        get { return m_blurFactor; }
        set { m_blurFactor = value;_UpdateMaterial(); }
    }
    #endregion

    private void Start()
    {
        if (m_material == null)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer==null)
            {
                Debug.LogWarning("Cannot find a renderer");
                return;
            }
            m_material = renderer.sharedMaterial;
        }
        _UpdateMaterial();
    }

    private void _UpdateMaterial()
    {
        if (m_material!=null)
        {
            m_generatedTexture = _GenerateProceduralTexture();
            m_material.SetTexture("m_MainTex", m_generatedTexture);
        }
    }
    private Color _MixColor(Color color0, Color color1, float mixFactor)
    {
        Color mixColor = Color.white;
        mixColor.r = Mathf.Lerp(color0.r, color1.r, mixFactor);
        mixColor.g = Mathf.Lerp(color0.g, color1.g, mixFactor);
        mixColor.b = Mathf.Lerp(color0.b, color1.b, mixFactor);
        mixColor.a = Mathf.Lerp(color0.a, color1.a, mixFactor);
        return mixColor;
    }
    private Texture2D _GenerateProceduralTexture()
    {
        Texture2D proceduralTexture = new Texture2D(textureWidth, textureWidth);

        //圆与圆之间间距
        float circleInterval = textureWidth / 4.0f;
        //圆的半径
        float radius = textureWidth / 10.0f;
        //模糊系数
        float edgeBlur = 1.0f / blurFactor;

        for (int w = 0; w < textureWidth; w++)
        {
            for (int h = 0; h < textureWidth; h++)
            {
                //初始化背景颜色
                Color pixel = backgroundColor;

                //依次画9个圆
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        //计算当前所绘制的圆的圆心位置
                        Vector2 circleCenter = new Vector2(circleInterval * (i + 1), circleInterval * (j + 1));
                        //计算当前像素与圆心的距离
                        float dist = Vector2.Distance(new Vector2(w, h), circleCenter) - radius;
                        //模糊圆的边界
                        Color color = _MixColor(circleColor, new Color(pixel.r, pixel.g, pixel.b, 0.0f), Mathf.SmoothStep(0f, 1.0f, dist * edgeBlur));
                        //与之前得到的颜色进行混合
                        pixel = _MixColor(pixel, color, color.a);
                    }
                }

                proceduralTexture.SetPixel(w, h, pixel);
            }
        }
        proceduralTexture.Apply();
        return proceduralTexture;
    }
}


public class SetPropertyAttribute : PropertyAttribute
{
    public string Name { get; private set; }
    public bool IsDirty { get; set; }

    public SetPropertyAttribute(string name)
    {
        this.Name = name;
    }
}
[CustomPropertyDrawer(typeof(SetPropertyAttribute))]
public class SetPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Rely on the default inspector GUI
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label);

        // Update only when necessary
        SetPropertyAttribute setProperty = attribute as SetPropertyAttribute;
        if (EditorGUI.EndChangeCheck())
        {
            // When a SerializedProperty is modified the actual field does not have the current value set (i.e.  
            // FieldInfo.GetValue() will return the prior value that was set) until after this OnGUI call has completed. 
            // Therefore, we need to mark this property as dirty, so that it can be updated with a subsequent OnGUI event 
            // (e.g. Repaint)
            setProperty.IsDirty = true;
        }
        else if (setProperty.IsDirty)
        {
            // The propertyPath may reference something that is a child field of a field on this Object, so it is necessary
            // to find which object is the actual parent before attempting to set the property with the current value.
            object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
            Type type = parent.GetType();
            PropertyInfo pi = type.GetProperty(setProperty.Name);
            if (pi == null)
            {
                Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
            }
            else
            {
                // Use FieldInfo instead of the SerializedProperty accessors as we'd have to deal with every 
                // SerializedPropertyType and use the correct accessor
                pi.SetValue(parent, fieldInfo.GetValue(parent), null);
            }
            setProperty.IsDirty = false;
        }
    }

    private object GetParentObjectOfProperty(string path, object obj)
    {
        string[] fields = path.Split('.');

        // We've finally arrived at the final object that contains the property
        if (fields.Length == 1)
        {
            return obj;
        }

        // We may have to walk public or private fields along the chain to finding our container object, so we have to allow for both
        FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        obj = fi.GetValue(obj);

        // Keep searching for our object that contains the property
        return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
    }
}
#endif
