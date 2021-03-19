using UnityEngine;

namespace EditorLearn
{
    // Scene视图下绘制，Game视图下不起作用
    public class MyGizmos : MonoBehaviour
    {
        // 场景的每一帧调用
        private void OnDrawGizmos()
        {
        }

        public float areaRadius;
        public float lineSize;
        public Vector3[] nodePoints = new[] {Vector3.zero};

        // 点击附有该脚本的物体时调用
        private void OnDrawGizmosSelected()
        {
            // 绘制线框球体。参数1：位置    参数2：半径
            Gizmos.DrawWireSphere(transform.position, areaRadius);
            // 画线。参数1：起始点，参数2终止点
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineSize);
            // 绘制球体
            Gizmos.color = Color.cyan;
            for (int i = 0; i < nodePoints.Length; i++)
            {
                Gizmos.DrawSphere(nodePoints[i], 0.5f);
            }
            // 绘制图片
            Gizmos.DrawIcon(transform.position, "dianping.jpg");
        }
    }
}