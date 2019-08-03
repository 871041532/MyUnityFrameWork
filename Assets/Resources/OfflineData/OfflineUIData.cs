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
        int count = m_AllPoint.Length;
        for (int i = 0; i < count; i++)
        {
            RectTransform temp = m_AllPoint[i] as RectTransform;
            if (!(temp is null))
            {
                temp.localPosition = m_Pos[i];
                temp.localRotation = m_Rot[i];
                temp.localScale = m_Scale[i];
                temp.anchorMax = m_AnchorMax[i];
                temp.anchorMin = m_AnchorMin[i];
                temp.pivot = m_Pivot[i];
                temp.sizeDelta = m_SizeDelta[i];
                temp.anchoredPosition = m_AnchoredPos[i];
            }

            int particleCount = m_Particle.Length;
            for (i = 0; i < particleCount; i++)
            {
                m_Particle[i].Clear(true);
                m_Particle[i].Play();
            }
        }
    }

    public override void BindDataInEditor()
    {
        Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
        int count = trans.Length;
        for (int i = 0; i < count; i++)
        {
            if (!(trans[i] is RectTransform))
            {
                trans[i].gameObject.AddComponent<RectTransform>();
            }
        }
        m_AllPoint = gameObject.GetComponentsInChildren<RectTransform>(true);
        int allPointCount = m_AllPoint.Length;
        m_AllPointChildCount = new int[allPointCount];
        m_AllPointActive = new bool[allPointCount];
        m_Pos = new Vector3[allPointCount];
        m_Scale = new Vector3[allPointCount];
        m_Rot = new Quaternion[allPointCount];
        m_AnchorMax = new Vector2[allPointCount];
        m_AnchorMin = new Vector2[allPointCount];
        m_Pivot = new Vector2[allPointCount];
        m_SizeDelta = new Vector2[allPointCount];
        m_AnchoredPos = new Vector3[allPointCount];
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < allPointCount; i++)
        {
            RectTransform temp = m_AllPoint[i] as RectTransform;
            m_AllPointChildCount[i] = temp.childCount;
            m_AllPointActive[i] = temp.gameObject.activeSelf;
            m_Pos[i] = temp.localPosition;
            m_Scale[i] = temp.localScale;
            m_Rot[i] = temp.localRotation;

            m_AnchorMax[i] = temp.anchorMax;
            m_AnchorMin[i] = temp.anchorMin;
            m_Pivot[i] = temp.pivot;
            m_SizeDelta[i] = temp.sizeDelta;
            m_AnchoredPos[i] = temp.anchoredPosition;
        }
    }
}
