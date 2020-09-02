using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathLearn;

public class GenerateMatrixTest : MonoBehaviour
{
    [Header("平移")]
    public UnityEngine.Vector3 m_Offset;

    [Header("缩放")]
    public UnityEngine.Vector3 m_Scale;

    [Header("旋转")]
    public UnityEngine.Vector3 m_Rotate;
    
    [Header("先缩放再旋转")]
    public bool isFirsScale = true;

    private Material m_Mat;

    // Start is called before the first frame update
    void Start()
    {
        m_Mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Matrix44 scale = new Matrix44();
        scale[0, 0] =  m_Scale.x;
        scale[1, 1] =  m_Scale.y;
        scale[2, 2] =  m_Scale.z;
        scale[3, 3] = 1;
        
        Matrix44 rotateHeading = Matrix44.AngleAxis(m_Rotate.y, new MathLearn.Vector3(0, 1, 0)).Inverse();
        Matrix44 rotatePitch = Matrix44.AngleAxis(m_Rotate.x, new MathLearn.Vector3(1, 0, 0)).Inverse();
        Matrix44 rotateBank = Matrix44.AngleAxis(m_Rotate.z, new MathLearn.Vector3(0, 0, 1)).Inverse();
        Matrix44 rotate = rotateHeading * rotatePitch * rotateBank;
        
        Matrix44 translate = Matrix44.Translate(new MathLearn.Vector3(m_Offset.x, m_Offset.y, m_Offset.z));

        // 世界转局部
        var temp1 = translate.Inverse() * rotateHeading * rotatePitch * rotateBank * scale.Inverse();
        // 局部转世界
        var temp2 = temp1.Inverse();
        var temp3 = scale * rotateBank.Inverse() * rotatePitch.Inverse() * rotateHeading.Inverse() * translate;
        var a = temp3.MulVector(new MathLearn.Vector3(1, 0, 0)) ;
        var b = temp3.MulVector(new MathLearn.Vector3(0, 1, 0)) ;
        var c = temp3.MulVector(new MathLearn.Vector3(0, 0, 1)) ;
        
        if (isFirsScale)
        {
            // 先缩放再旋转
            Matrix44 m =  temp1;
            m = m.Inverse();
            m_Mat.SetColor("m_Line1", new Color(m[0, 0], m[0,1], m[0,2], m[0,3]));   
            m_Mat.SetColor("m_Line2",  new Color(m[1,0], m[1,1], m[1,2], m[1,3]));   
            m_Mat.SetColor("m_Line3",  new Color(m[2,0], m[2,1], m[2,2], m[2,3]));   
            m_Mat.SetColor("m_Line4",  new Color(m[3,0], m[3,1], m[3,2], m[3,3]));   
        }
        else
        {
            // 先旋转再缩放
            Matrix44 m =  rotate * scale *  translate;
            m = m.Inverse();
            m_Mat.SetColor("m_Line1", new Color(m[0,0], m[0,1], m[0,2], m[0,3]));   
            m_Mat.SetColor("m_Line2",  new Color(m[1,0], m[1,1], m[1,2], m[1,3]));   
            m_Mat.SetColor("m_Line3",  new Color(m[2,0], m[2,1], m[2,2], m[2,3]));   
            m_Mat.SetColor("m_Line4",  new Color(m[3,0], m[3,1], m[3,2], m[3,3]));
        }
      
    }
}
