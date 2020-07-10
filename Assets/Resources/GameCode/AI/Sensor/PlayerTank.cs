using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerTank : MonoBehaviour
{
    public Transform m_targetTransform;

    public float m_targetDistanceTolerance = 3.0f;

    private float m_movementSpeed = 10.0f;
    private float m_rotationSpeed = 2.0f;
    
    void Update()
    {
        Vector3 targetPosition = m_targetTransform.position;
        targetPosition.y = transform.position.y;
        Vector3 direction = targetPosition - transform.position;
        if (direction.magnitude< m_targetDistanceTolerance)
        {
            return;
        }
        Quaternion tarRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, m_rotationSpeed * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, m_movementSpeed * Time.deltaTime));
    }
}
