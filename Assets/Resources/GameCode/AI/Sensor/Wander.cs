using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private Vector3 m_targetPosition;
    
    private float m_movementSpeed = 5.0f;
    private float m_rotationSpeed = 2.0f;
    private float m_targetPositionTolerance = 3.0f;

    private float m_minX = -30.0f;
    private float m_maxX = 30.0f;
    private float m_minZ = -30.0f;
    private float m_maxZ = 30.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        CalculateNextPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(m_targetPosition, transform.position) <= m_targetPositionTolerance)
        {
            CalculateNextPosition();
        }

        Quaternion targetRotation = Quaternion.LookRotation(m_targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
        
        transform.Translate(new Vector3(0, 0, m_movementSpeed * Time.deltaTime));
    }

    private void CalculateNextPosition()
    {
        m_targetPosition = new Vector3(Random.Range(m_minX, m_maxX), 0.5f, Random.Range(m_minZ, m_maxZ));
    }
}
