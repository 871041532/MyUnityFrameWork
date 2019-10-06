using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Flock
{
    public class FlockController : MonoBehaviour
    {
        [SerializeField] private int m_flockSize = 20;
        
        // 飞行速度
        public float m_speedModifier = 5;
        // 对齐力
        [SerializeField] private float m_aligmentWeight = 1;
        // 内聚力
        [SerializeField] private float m_cohesionWeight = 1;
        // 间隔力
        [SerializeField] private float m_separationWeight = 1;
        // 跟随目标点远近力
        [SerializeField] private float m_followWeight = 5;

        [SerializeField] private Boid m_prefab;

        [SerializeField] private float m_spawnRadius = 3;

        private Vector3 m_spawnLocation = Vector3.zero;
        // 目标点
        public Transform m_target;

        private List<Boid> m_flockList = null;
        private void Awake()
        {
            m_flockList = new List<Boid>(m_flockSize);
            for (int i = 0; i < m_flockSize; i++)
            {
                m_spawnLocation = UnityEngine.Random.insideUnitSphere * m_spawnRadius + transform.position;
                Boid boid = Instantiate(m_prefab, m_spawnLocation, transform.rotation) as Boid;;
                boid.transform.parent = transform;
                boid.FlockController = this;
                m_flockList.Add(boid);
            }
        }

        public Vector3 Flock(Boid boid, Vector3 boidPosition, Vector3 boidDirection)
        {
            var flockDirection = Vector3.zero;
            var flockCenter = Vector3.zero;
            var targetDirection = Vector3.zero;
            var separation = Vector3.zero;
            for (int i = 0; i < m_flockSize; i++)
            {
                Boid neighbor = m_flockList[i];
                if (neighbor != boid)
                {
                    flockDirection += neighbor.Direction;
                    flockCenter += neighbor.transform.localPosition;
                    // separation 模型1
                    var temp = - neighbor.transform.localPosition + boidPosition;
                    var prop = temp.magnitude / m_aligmentWeight;
                    temp.Normalize();
                    temp = temp * (1 / prop);
                    separation += temp;
                    // separation 模型2
//                    separation += neighbor.transform.localPosition - boidPosition;
//                    separation *= -1; 
                }
            }

            flockDirection /= m_flockSize;
            flockDirection = flockDirection.normalized * m_aligmentWeight;
            flockCenter /= m_flockSize;
            flockCenter = flockCenter.normalized * m_cohesionWeight;
            separation /= m_flockSize;
            separation = separation.normalized * m_separationWeight;
            targetDirection = m_target.localPosition - boidPosition;
            targetDirection = targetDirection * m_followWeight;
            return flockDirection + flockCenter + separation + targetDirection;
        }
    }
}