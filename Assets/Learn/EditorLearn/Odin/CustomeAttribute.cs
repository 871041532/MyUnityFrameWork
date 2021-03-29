// 自定义血量进度条属性float，用红色的Bar来展示血量
using System;
using Sirenix.OdinInspector;

public class HealthBarAttribute:Attribute
{
    public float MaxHealth;
    public HealthBarAttribute(float maxHealth)
    {
        this.MaxHealth = maxHealth;
    }       
}


// 自定义类的Attribute，加入标签之后可以绘制
[Serializable] // The Serializable attributes tells Unity to serialize fields of this type.
public struct MyStruct
{
    public float X;
    public float Y;
}

public class MyStructAttribute:Attribute
{  
}

// 自定义Group布局的Attribute，实现自定义带颜色的折叠区域
public class ColoredFoldoutGroupAttribute : PropertyGroupAttribute
{
    public float r, g, b, a;
    public ColoredFoldoutGroupAttribute(string path) : base(path)
    {
    }

    public ColoredFoldoutGroupAttribute(string path, float r, float g, float b, float a = 1) : base(path)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
   
    // This method is called for all attributes with the same path, and this is where we will handle merging the colour values as mentioned earlier.
    protected override void CombineValuesWith(PropertyGroupAttribute other)
    {
        var otherAttr = (ColoredFoldoutGroupAttribute)other;
        this.r = Math.Max(otherAttr.r, this.r);
        this.g = Math.Max(otherAttr.g, this.g);
        this.b = Math.Max(otherAttr.b, this.b);
        this.a = Math.Max(otherAttr.a, this.a);
    }
}

