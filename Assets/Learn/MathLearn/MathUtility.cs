using System;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public static class MathUtility
    {
        // 计算3D中任意点的中心坐标
        public static void ComputeBarycentricCoords3d(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 p, out Vector3 b)
        { 
            b = new Vector3(1, 2, 3);
            var a = 1;
        }
    }
}