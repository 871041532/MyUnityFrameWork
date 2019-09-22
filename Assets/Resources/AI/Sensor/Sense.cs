using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Sense : MonoBehaviour
{
    public bool m_enableDebug = true;

    public Aspect.AspectType m_aspectType = Aspect.AspectType.ENEMY;

    public float m_detectionRate = 1.0f;

    protected float m_elapseTime = 0;

    protected virtual void OnInit()
    {
        
    }

    protected virtual void OnUpdateSense()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        OnInit();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdateSense();
    }
}
