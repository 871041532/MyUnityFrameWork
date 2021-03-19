using UnityEngine;

namespace EditorLearn
{
    // 在场景中绘线的数据类
    public class MyHandles: MonoBehaviour
    {
        public float areaRadius;
        public float size;
        public Vector3[] nodePoints = new Vector3[]{};
        public Quaternion[] nodeRotations = new Quaternion[]{};
        public bool showHandles = true;
    }
}