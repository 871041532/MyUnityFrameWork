using UnityEngine;
using UVector3 = UnityEngine.Vector3;
using MVector3 = MathLearn.Vector3;
using System;

namespace MathLearn
{
    public class ClipMatrixTest : MonoBehaviour
    {
        Camera m_mainCamera;
        Matrix4x4 m_unityClipMatrix;
      
        // Use this for initialization
        void Start()
        {
            // 获取Unity主相机的矩阵
            m_mainCamera = Camera.main;
            m_unityClipMatrix = m_mainCamera.projectionMatrix * m_mainCamera.worldToCameraMatrix;
            UVector3 testPoint = UVector3.one;
            Debug.Log(m_unityClipMatrix * testPoint);
            // 使用自定义数学库，实现世界坐标到相机空间坐标矩阵（不适用Unity的右手坐标系，而是用左手坐标系）
            var forward = m_mainCamera.transform.forward;
            var right = m_mainCamera.transform.right;
            var up = m_mainCamera.transform.up;
            Matrix44 rotation = new Matrix44(right.x, up.x, forward.x, 0, right.y, up.y, forward.y, 0, right.z, up.z, forward.z, 0, 0, 0, 0, 1);
            var cameraPosition = m_mainCamera.transform.position;
            Matrix44 translate = new Matrix44(1,0,0,0,  0,1,0,0,  0,0,1,0, -cameraPosition.x, -cameraPosition.y, -cameraPosition.z,1);
            Matrix44 mWorldToCameraMatrix = translate * rotation;
            // 自定义库，实现相机空间坐标到裁剪坐标转换
            float fov = m_mainCamera.fieldOfView;
            float halfAngle = fov * (float)Math.PI / 360f;
            float cot = 1 / (float)Math.Tan(halfAngle);
            float aspect = m_mainCamera.aspect;
            float n = m_mainCamera.nearClipPlane;
            float f = m_mainCamera.farClipPlane;
            Matrix44 mCameraToClipMatrix = new Matrix44(cot/aspect,0,0,0, 0,cot,0,0, 0,0,(f+n)/(f-n),1, 0,0,2*n*f/(n-f),0);
            // 直接从世界空间变换到裁剪空间，两个值除去误差是一样的
            var pu = m_unityClipMatrix * UnityEngine.Vector4.one;
            var pm = MathLearn.Vector4.one * mWorldToCameraMatrix * mCameraToClipMatrix;
            
        }
    }
}