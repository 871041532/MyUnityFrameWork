using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : Sense
{
    private void OnTriggerEnter(Collider other)
    {
        Aspect aspect = other.GetComponent<Aspect>();
        if (aspect != null && aspect.m_aspectType != m_aspectType)
        {
            print("Enemy Touch Detected!");
        }
    }

    private void OnDrawGizmos()
    {
        var colider = gameObject.GetComponent<BoxCollider>();
        Vector3 center = colider.center;
        Vector3 size = colider.size;
        var position = transform.position;
        Debug.DrawLine(position, new Vector3(position.x - size.x * 0.5f, 0, position.z - size.z * 0.5f), new Color(1f, 0.83f, 0.24f));
        Debug.DrawLine(position, new Vector3(position.x + size.x * 0.5f, 0, position.z + size.z * 0.5f), new Color(1f, 0.83f, 0.24f));
        Debug.DrawLine(position, new Vector3(position.x - size.x * 0.5f, 0, position.z + size.z * 0.5f), new Color(1f, 0.83f, 0.24f));
        Debug.DrawLine(position, new Vector3(position.x + size.x * 0.5f, 0, position.z - size.z * 0.5f), new Color(1f, 0.83f, 0.24f));
    }
}
