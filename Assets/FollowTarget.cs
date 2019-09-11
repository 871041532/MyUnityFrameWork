using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform m_Target;
    // Start is called before the first frame update
    private Vector3 m_localPosition = new Vector3(0.1f, 1.9f, -5.6f);
    void Start()
    {
//        m_localPosition = m_Target.InverseTransformPoint(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_Target.TransformPoint(m_localPosition);
        transform.rotation = m_Target.rotation;
    }
}
