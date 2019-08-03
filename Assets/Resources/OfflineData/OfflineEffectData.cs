using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineEffectData : OfflineData
{
    public ParticleSystem[] m_Particle;
    public TrailRenderer[] m_TrailRender;  // 拖尾

    public override void ResetProperty()
    {
        base.ResetProperty();
        foreach (ParticleSystem item in m_Particle)
        {
            item.Clear(true);
            item.Play();
        }

        foreach (TrailRenderer item in m_TrailRender)
        {
            item.Clear();
        }
    }

    public override void BindDataInEditor()
    {
        base.BindDataInEditor();
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        m_TrailRender = gameObject.GetComponentsInChildren<TrailRenderer>(true);
    }

}
