using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineUIData : OfflineData
{
    public Vector2[] m_AnchorMax;
    public Vector2[] m_AnchorMin;
    public Vector2[] m_Pivot;
    public Vector2[] m_SizeDelta;
    public Vector3[] m_AnchoredPos;
    public ParticleSystem[] m_Particle;

    public override void ResetProperty()
    {
    }

    public override void BindDataInEditor()
    {
    }
}
