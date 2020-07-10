using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perspective : Sense
{
    public int m_fieldOfView = 45;
    public int m_viewDistance = 100;
    public Transform m_playerTransform = null;
    private Vector3 m_rayDirection;

    protected override void OnInit()
    {
        m_playerTransform = GameObject.FindWithTag("Player").transform;
    }

    protected override void OnUpdateSense()
    {
        m_elapseTime += Time.deltaTime;
        if (m_elapseTime >= m_detectionRate)
        {
            DetectAspect();
            m_elapseTime = 0;
        }
    }

    void DetectAspect()
    {
        RaycastHit hit;
        m_rayDirection = m_playerTransform.position - transform.position;
        if ((Vector3.Angle(m_rayDirection, transform.forward)) < m_fieldOfView)
        {
            if (Physics.Raycast(transform.position, m_rayDirection, out hit, m_viewDistance))
            {
                Aspect aspect = hit.collider.GetComponent<Aspect>();
                if (!(aspect is null))
                {
                    if (aspect.m_aspectType != m_aspectType)
                    {
                        Debug.Log("Enemy Detected!");
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_playerTransform == null)
        {
            return;
        }

        var position = transform.position;
        Debug.DrawLine(position, m_playerTransform.position, new Color(1f, 0.83f, 0.24f));
        Vector3 frontRayPoint = position + transform.forward * m_viewDistance;
        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += m_fieldOfView * 0.5f;
        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= m_fieldOfView * 0.5f;
        Debug.DrawLine(position, frontRayPoint, new Color(1f, 0.83f, 0.24f));
        Debug.DrawLine(position, leftRayPoint, new Color(1f, 0.83f, 0.24f));
        Debug.DrawLine(position, rightRayPoint, new Color(1f, 0.83f, 0.24f));
    }
}
