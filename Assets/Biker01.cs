using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biker01 : MonoBehaviour
{
    public Animator m_Animator;

    public Transform m_Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // OnAnimatorIK由系统调用，自动调用
    private void OnAnimatorIK(int layerIndex)
    {
        // 设置IK
        m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_Target.position);
        // 设置权重 0 ~ 1
        m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
    }
}
