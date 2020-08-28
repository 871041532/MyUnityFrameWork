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
    public float mAngle = 3f;

    private MVector3 mAxis = new MVector3(0, 0, 1);

    private Mesh mMesh;

    private UnityEngine.Vector3[] mVertices;
    
    // Start is called before the first frame update
    void Start()
    {
        var p = new MVector3(1, 0, 0);        
        Matrix33 rotation = Matrix33.AngleAxis(45, new MVector3(0, 0, 1));
        Matrix33 rotation2 = Matrix33.AngleAxis(-45, new MVector3(0, 0, 1));
        Debug.Log(p * rotation);
        Debug.Log(p * rotation * rotation2);
        Debug.Log(p * (rotation * rotation2));

        Debug.Log("-------------------------------------------------------------");
        Matrix44 mul = Matrix44.AngleAxisTranslate(45, new MVector3(0, 0, 1), new MVector3(1, 1, 0));
        Debug.Log(p * mul);
        Debug.Log(p * (mul * mul.Inverse()));
        
        Debug.Log("-------------------------------------------------------------");
        Matrix44 r = Matrix44.AngleAxis(45, new MVector3(0, 0, 1));
        Matrix44 t = Matrix44.Translate(new MVector3(1, 1, 0));
        Matrix44 composite = r * t;
        Matrix44 composite2 = t.Inverse() * r.Inverse();
        Debug.Log(p * composite);
        Debug.Log(p * composite * composite2);
        Debug.Log(p * composite * composite.Inverse());
        
        mMesh = GetComponent<MeshFilter>().mesh;
        mVertices = mMesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        Matrix44 rotate = Matrix44.AngleAxis(mAngle, mAxis);
        for (int i = 0; i < mVertices.Length; i++)
        {
            var item = mVertices[i];
            var v = new MVector3(item.x, item.y, item.z);
            var t = v * rotate;
            mVertices[i].x = t.x;
            mVertices[i].y = t.y;
            mVertices[i].z = t.z;
        }
        mMesh.vertices = mVertices;
    }
}
