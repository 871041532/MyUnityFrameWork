using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineData : MonoBehaviour
{
    public Rigidbody m_Rigidbody;  // 刚体
    public Collider m_Collider;  // 碰撞体
    public Transform[] m_AllPoint;  // 所有子节点
    public int[] m_AllPointChildCount;  // 子节点的子节点数目
    public bool[] m_AllPointActive;  // 子节点avtive
    public Vector3[] m_Pos;  // 子节点位置信息
    public Vector3[] m_Scale;  // 子节点缩放信息
    public Quaternion[] m_Rot;  // 子节点旋转信息

    public virtual void ResetProperty()
    {
        int allPointCount = m_AllPoint.Length;
        for (int i = 0; i < allPointCount; i++)
        {
            Transform temp = m_AllPoint[i];
            temp.localPosition = m_Pos[i];
            temp.localRotation = m_Rot[i];
            temp.localScale = m_Scale[i];
            if (m_AllPointActive[i] != temp.gameObject.activeSelf)
            {
                temp.gameObject.SetActive(m_AllPointActive[i]);
            }
            if (temp.childCount > m_AllPointChildCount[i])
            {
                int childCount = temp.childCount;
                for (int j = m_AllPointChildCount[i]; j < childCount; j++)
                {
                    GameObject tempObj = temp.GetChild(j).gameObject;
                    GameObject.Destroy(tempObj);
                }
            }
        }
    }

    public virtual void BindDataInEditor()
    {
        // true隐藏的也找
        m_Collider = gameObject.GetComponentInChildren<Collider>(true);
        m_Rigidbody = gameObject.GetComponentInChildren<Rigidbody>(true);  
        m_AllPoint = gameObject.GetComponentsInChildren<Transform>(true);
        int allPointCount = m_AllPoint.Length;
        m_AllPointChildCount = new int[allPointCount];
        m_AllPointActive = new bool[allPointCount];
        m_Pos = new Vector3[allPointCount];
        m_Scale = new Vector3[allPointCount];
        m_Rot = new Quaternion[allPointCount];
        for (int i = 0; i < allPointCount; i++)
        {
            Transform temp = m_AllPoint[i];
            m_AllPointChildCount[i] = temp.childCount;
            m_AllPointActive[i] = temp.gameObject.activeSelf;
            m_Pos[i] = temp.localPosition;
            m_Scale[i] = temp.localScale;
            m_Rot[i] = temp.localRotation;
        }
    }
}
