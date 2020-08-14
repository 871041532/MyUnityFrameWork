using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathLearn;

public class MathLearnTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        var v = new MathLearn.Vector3(1, 2, 3);
        var n = new MathLearn.Vector3(4, 5, 6);
        Debug.Log(n.Magnitude() * n.Magnitude());
        Debug.Log(n * n);
        var target1 = n * (v * n / (n.Magnitude() * n.Magnitude()));
        Debug.Log($"{target1.x}，{target1.y}，{target1.z}");
        var target2 = v *  n * n / (n.Magnitude() * n.Magnitude());
        Debug.Log($"{target2.x}，{target2.y}，{target2.z}");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
