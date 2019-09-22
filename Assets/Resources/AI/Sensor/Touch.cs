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
    
}
