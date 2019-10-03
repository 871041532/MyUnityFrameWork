using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public bool m_isDebug;
    [SerializeField]
    private Vector3[] m_wayPoints;
    [SerializeField]
    private float m_tolerance = 3.0f;

    public float Tolerance
    {
        get { return m_tolerance; }
    }
    public float Count
    {
        get { return m_wayPoints.Length; }
    }

    public Vector3 GetPoint(int index)
    {
        return m_wayPoints[index];
    }

    private void OnDrawGizmos()
    {
        if (m_isDebug)
        {
            for (int i = 0; i < m_wayPoints.Length; i++)
            {
                if (i+1 < m_wayPoints.Length)
                {
                    Debug.DrawLine(m_wayPoints[i], m_wayPoints[i+1], Color.red);
                }
            }
        }
    }
}
