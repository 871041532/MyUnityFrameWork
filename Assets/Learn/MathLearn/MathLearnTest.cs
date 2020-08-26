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
        var q1 = new MQuaternion(0, 1f, 0, 0);
        var p = new MVector3(1f, 0, 0);
        var p2 = q1 * p;
        Debug.Log(p2);

        var q2 = MQuaternion.AngleAxis(180, new MVector3(0, 1, 0));
        print(q2);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
