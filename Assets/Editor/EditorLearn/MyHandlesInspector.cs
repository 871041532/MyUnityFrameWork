using UnityEditor;
using UnityEngine;

namespace EditorLearn
{
    // 在场景中画线的编辑器类, 只有被选中的物体才会在Scene中绘制。
    // Handles与Gizmos区别在于，Handles绘制的图像只针对自身被选中的情况，而且可以操作。
    // 而Gizmos可以当自身被选中时绘制，也可以忽略选中一直绘制，但是不可以操作
    // 此外Gizmos必须在MonoBehaviour脚本的OnDrawGizmosSelected、或者OnDrawGizmos中调用， 而Handles脚本是在Editor脚本中被调用
    [CustomEditor(typeof(MyHandles))]
    public class MyHandlesInspector : Editor
    {
        private MyHandles myHandle;

        private void OnEnable()
        {
            myHandle = target as MyHandles;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            // 给场景绘制操作柄加上开关
            Handles.BeginGUI();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("开打/关闭操作柄"))
            {
                myHandle.showHandles = !myHandle.showHandles;
            }
            GUILayout.EndHorizontal();
            Handles.EndGUI();
            
            /*
            *  绘制线，用于连接操作手柄
            */
            Handles.color = Color.blue;
            for (int i = 0; i < myHandle.nodePoints.Length - 1; i++)
            {
                var pointStart = myHandle.nodePoints[i];
                var pointEnd = myHandle.nodePoints[i + 1];
                Handles.DrawLine(pointStart, pointEnd);
            } 
            
            //用于在场景中显示设置的名字
            //第一个参数为在场景中显示的位置(以物体的中心位置为基准)
            //第二个参数为显示的名字
            for (int i = 0; i < myHandle.nodePoints.Length; i++)
            {
                Handles.Label(myHandle.nodePoints[i] + Vector3.up * 0.5f, "点: " + i.ToString()); 
            }

            //用于在场景中绘制半径操作柄
            //第一个参数为该旋转操作柄的初始旋转角度
            //第二个参数为操作柄显示的位置(以物体的旋转位置为基准)
            //第三个参数为设置操作柄的半径
            myHandle.areaRadius =
                Handles.RadiusHandle(myHandle.transform.rotation, myHandle.transform.position, myHandle.areaRadius);

            // 在场景中绘制图形
            /**
             * 参数1：尺寸，返回的也是这个尺寸值
             * 参数2：位置
             * 参数3：旋转
             * 参数4：操作柄的大小
             * 参数5：形状
             * Handles.CircleHandleCap 圆形
             *  Handles.ArrowHandleCap 箭头
             * Handles.ConeHandleCap 圆锥体
             * Handles.CubeHandleCap 立方体
             * Handles.CylinderHandleCap 圆柱体
             * Handles.DotHandleCap 矩形点
             * Handles.RectangleHandleCap 矩形
             * Handles.SphereHandleCap 球形
             */
            Handles.color = Color.cyan;
            for (int i = 0; i < myHandle.nodePoints.Length; i++)
            {
                myHandle.size = Handles.ScaleValueHandle(myHandle.size, myHandle.nodePoints[i], myHandle.nodeRotations[i], myHandle.size, Handles.ConeHandleCap, 0.5f);
            }   
            
            if (!myHandle.showHandles)
            {
                return;
            }
            
            /* 场景中绘制位置操作柄
             *  参数1：操作柄位于世界坐标的位置
             *   参数2：操作柄的旋转方向
             */ 
            for (int i = 0; i < myHandle.nodePoints.Length; i++)
            {
                myHandle.nodePoints[i] = Handles.PositionHandle(myHandle.nodePoints[i], Quaternion.identity);
            }
            
            /* 场景中绘制旋转操作柄
            *  参数1：操作柄位于世界坐标的位置
            *   参数2：操作柄的旋转方向
            */ 
            for (int i = 0; i < myHandle.nodeRotations.Length; i++)
            {
                myHandle.nodeRotations[i] = Handles.RotationHandle(myHandle.nodeRotations[i],myHandle.nodePoints[i]);
            }
            
        }
    }
}