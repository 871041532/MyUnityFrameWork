using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathLearn;
using UnityEditor;
using MVector3 = MathLearn.Vector3;
using MQuaternion = MathLearn.Quaternion;
using MVector2 = MathLearn.Vector2;
using MRay2D = MathLearn.Ray2D;
using MRay = MathLearn.Ray;
using Bounds2D = MathLearn.Bounds2D;
using MBounds = MathLearn.Bounds;
using Bounds = UnityEngine.Bounds; 
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using MPlane = MathLearn.Plane;

public class MathLearnTest : MonoBehaviour
{
    public float mAngle = 2f;

    private MVector3 mAxis = new MVector3(0, 0, 1);

    private Mesh mMesh;

    private UnityEngine.Vector3[] mVertices;
    private UnityEngine.Vector3[] mNormals;
    
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
        mNormals = mMesh.normals;


        var temp = new Matrix33(10, 0, 0, 0, 10, 0, 0, 0, 1);

        var temp2 = temp.Inverse();
        var tempInverse = new Matrix33(0,0,0, 0,0,0, -3, -4, 1);
        p = new MVector3(0, 0, 1);
        p = p * temp;
        p.z = 1;
        p = p * tempInverse;
        var t3 = temp * tempInverse;

        t = Matrix44.Translate(new MVector3(3, 4, 5));
        r = Matrix44.AngleAxis(30, new MVector3(0, 1, 0));
        var s = Matrix44.Scale(new MVector3(0.5f, 2, 1));

        p = new MVector3(1, 1, 1);
        var p1 = p * s * r * t;
        p = p1 * t.Inverse() * r.Inverse() * s.Inverse();

        var ray2d1 = new MRay(new MVector3(1, -1, 0), new MVector3(1, 1, 0));

        var aaa = ray2d1.GetNearestPos(new MVector3(1, 1, 0));
        float aaaa = 1;

        var plane1 = new MPlane(new MVector3(1,1,1), 1);
        var pos1 = plane1.GetNearestPos(new MVector3(5, 5, 5));
        var pos2 = plane1.GetNearestPos(new MVector3(-5, -5, -5));
        
        var plane2 = new MPlane(new MVector3(-1,-1,-1), -1);
        var pos3 = plane2.GetNearestPos(new MVector3(5, 5, 5));
        var pos4 = plane2.GetNearestPos(new MVector3(-5, -5, -5));
        
        var plane3 = new MPlane(new MVector3(-1,-1,-1), 1);
        var pos5 = plane3.GetNearestPos(new MVector3(5, 5, 5));
        var pos6 = plane3.GetNearestPos(new MVector3(-5, -5, -5));

        var tempxxx = 1;
    }

    // Update is called once per frame
    void Update()
    {

        float minX = Random.Range(0, 10);
        float minY = Random.Range(0, 10);
        float minZ = Random.Range(0, 10);
        
        float maxX = Random.Range(10.1f, 20);
        float maxY = Random.Range(10.1f, 20);
        float maxZ = Random.Range(10.1f, 20);

        float originX = Random.Range(0, 50);
        float originY = Random.Range(0, 50);
        float originZ = Random.Range(0, 50);
        
        float directionX = Random.Range(1, 10);
        float directionY = Random.Range(1, 10);
        float directionZ = Random.Range(1, 10);
        
        MBounds myB = new MBounds(MVector3.zero, MVector3.zero);
        myB.SetMinMax(new MVector3(minX, minY, minZ), new MVector3(maxX, maxY, maxZ));
        float mD = 0;
        bool mR = myB.IntersectRay(new MRay(new MVector3(originX, originY, originZ), new MVector3(directionX, directionY, directionZ)), out mD);
        
        Bounds B = new Bounds(Vector3.zero, Vector3.zero);
        B.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        float D = 0;
        bool R = B.IntersectRay(new UnityEngine.Ray(new Vector3(originX, originY, originZ), new Vector3(directionX, directionY, directionZ)), out D);

        if (Mathf.Abs(mD - D) < 0.00001f && mR == R)
        {
        }
        else
        {
            if(originX < minX || originX > maxX || originY < minY || originY > maxY || originZ < minZ || originZ > maxZ)
            {
                var a = 1;
            }
        }


        Matrix44 rotate = Matrix44.AngleAxis(mAngle, mAxis);
        for (int i = 0; i < mVertices.Length; i++)
        {
            var item = mVertices[i];
            var v = new MVector3(item.x, item.y, item.z);
            var t = v * rotate;
            mVertices[i].x = t.x;
            mVertices[i].y = t.y;
            mVertices[i].z = t.z;
            
            var item2 = mNormals[i];
            var v2 = new MVector3(item2.x, item2.y, item2.z);
            var t2 = v2 * rotate;
            mNormals[i].x = t2.x;
            mNormals[i].y = t2.y;
            mNormals[i].z = t2.z;
        }
        mMesh.vertices = mVertices;
        mMesh.normals = mNormals;
    }
}
