using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flock
{
    public class Boid : MonoBehaviour
    {
        [SerializeField]
        private FlockController m_flockController;

        private Vector3 m_targetDirection;

        private Vector3 m_direction;

        public FlockController FlockController
        {
            get { return m_flockController;}
            set { m_flockController = value; }
        }

        public Vector3 Direction
        {
            get { return m_direction; }
        }

        private void Start()
        {
            m_direction = transform.forward.normalized;
            if (m_flockController is null)
            {
                Debug.LogError("you must assign a flock controller!");
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_targetDirection = m_flockController.Flock(this, transform.localPosition, m_direction);
            if (m_targetDirection == Vector3.zero)
            {
                return;
            }

            m_direction = m_targetDirection.normalized;
            m_direction *= m_flockController.m_speedModifier;
            transform.Translate(m_direction * Time.deltaTime);
        }
    }
}

