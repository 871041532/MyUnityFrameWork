using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPathing : MonoBehaviour
{
    [SerializeField]
    private WaypointPath m_path;
    [SerializeField]
    private float m_speed = 2.0f;
    [SerializeField]
    private float m_mass = 5.0f;
    [SerializeField]
    private bool m_isLooping = true;

    // 当前帧速率
    private float m_currentSpeed;
    
    private int m_curPathIndex;

    private Vector3 m_targetPoint;
    
    // 当前帧向量速度的微分
    private Vector3 m_directionSpeed;

    private Vector3 m_targetDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        m_directionSpeed = transform.forward;
        m_curPathIndex = 0;
        m_targetPoint = m_path.GetPoint(m_curPathIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_path is null)
        {
            return;
        }

        m_currentSpeed = m_speed * Time.deltaTime;
        if (targetReached() && !setNextTarget())
        {
            return;
        }

        m_directionSpeed += getAcceleration(m_targetPoint);
        // Move the agent according to the direction
        transform.position += m_directionSpeed;
        // Rotate the agent towards the desired direction
        transform.rotation = Quaternion.LookRotation(m_directionSpeed);
    }

    private Vector3 getAcceleration(Vector3 target)
    {
        m_targetDirection = target - transform.position;
        m_targetDirection.Normalize();
        m_targetDirection *= m_currentSpeed;
        Vector3 steeringForce = m_targetDirection - m_directionSpeed;
        Vector3 acceleration = steeringForce / m_mass;
        return acceleration;
    }
    
    private bool targetReached()
    {
        return Vector3.Distance(transform.position, m_targetPoint) < m_path.Tolerance;
    }

    private bool setNextTarget()
    {
        bool success = false;
        if (m_curPathIndex < m_path.Count - 1)
        {
            m_curPathIndex++;
            success = true;
        }
        else
        {
            if (m_isLooping)
            {
                m_curPathIndex = 0;
                success = true;
            }
            else
            {
                success = false;
            }
        }

        m_targetPoint = m_path.GetPoint(m_curPathIndex);
        return success;
    }
    
}
