using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathLearn;
using UnityEditor;
using MVector3 = MathLearn.Vector3;
using MQuaternion = MathLearn.Quaternion;

public class MathLearnTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var p = new MVector3(1, 0, 0);
        float cos = (float)Math.Cos(45 * Math.PI / 180);
        float sin = (float)Math.Sin(45 * Math.PI / 180);
        var composite = new Matrix33(cos, sin, 0, -sin, cos, 0, 0, 0, 1);
        Debug.Log(composite.Determinant());
        Debug.Log(MVector3.Cross(new MVector3(cos, sin, 0), new MVector3(-sin, cos, 0)) * new MVector3(0, 0, 1));
        Debug.Log(p * Matrix22.Rotation(45));
        Debug.Log(p * composite);
        var temp = composite.Inverse();
        Debug.Log(p * composite * composite.Inverse());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
