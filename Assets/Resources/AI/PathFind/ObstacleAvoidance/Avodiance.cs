using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Avodiance : MonoBehaviour
{
    [SerializeField] 
    private float m_movementSpeed = 10.0f;

    [SerializeField] 
    private float m_rotationSpeed = 2.5f;

    [SerializeField] 
    private float m_force = 2.5f;

    [SerializeField] 
    private float m_minimumAvoidanceDistance = 2.0f;

    [SerializeField]
    private float m_toleranceRadius = 2.0f;

    private float m_currentSpeed;
    private Vector3 m_targetPoint;
    private RaycastHit m_mouseHit;
    private Camera m_mainCamera;
    private Vector3 m_direction;
    private Quaternion m_targetRotation;
    private RaycastHit m_avoidanceHit;
    private Vector3 m_hitNormal;
    
    // Start is called before the first frame update
    void Start()
    {
        m_mainCamera = Camera.main;
        m_targetPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        checkMouseInput();
        m_direction = m_targetPoint - transform.position;
        m_direction.Normalize();
        applyAvoidance(ref m_direction);
        if (Vector3.Distance(m_targetPoint, transform.position) < m_toleranceRadius)
        {
            return;;
        }

        m_currentSpeed = m_movementSpeed * Time.deltaTime;
        m_targetRotation = Quaternion.LookRotation(m_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, m_rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * m_currentSpeed;
    }

    void applyAvoidance(ref Vector3 direction)
    {
        int layerMask = 1 << 8;
        if (Physics.Raycast(transform.position, transform.forward, out m_avoidanceHit, m_minimumAvoidanceDistance, layerMask))
        {
            // 获取射线碰撞到的平面的法线
            m_hitNormal = m_avoidanceHit.normal;
            m_hitNormal.y = 0;
            direction = transform.forward + m_hitNormal * m_force;
        }
    }
    
    // 根据鼠标点击获取目标点
    void checkMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out m_mouseHit, 100.0f))
            {
                m_targetPoint = m_mouseHit.point;
            }
        }
    }
}
