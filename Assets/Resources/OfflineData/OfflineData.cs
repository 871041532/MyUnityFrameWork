using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineData : MonoBehaviour
{
    public Rigidbody m_Rigidbody;  // 刚体
    public Collider m_Collider;  // 碰撞体
    public Transform[] m_AllPoint;  // 子节点
    public int[] m_AllPointChildCount;  // 每一个子节点数目
    public bool[] m_AllPointActive;  // 每个子节点avtive
    public Vector3[] m_Pos;  // 子节点位置信息
    public Vector3[] m_Scale;  // 子节点缩放信息
    public Quaternion[] m_Rot;  // 子节点旋转信息

    public virtual void ResetProperty()
    {

    }

    public virtual void BindDataInEditor()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
