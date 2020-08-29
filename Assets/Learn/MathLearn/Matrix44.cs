using System;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix44
    {
        private float m00;
        private float m01;
        private float m02;
        private float m03;
        
        private float m10;
        private float m11;
        private float m12;
        private float m13;
        
        private float m20;
        private float m21;
        private float m22;
        private float m23;
        
        private float m30;
        private float m31;
        private float m32;
        private float m33;
        private static int mNumber = 4;

        // 用离散的float数造矩阵
        public Matrix44(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;
            this.m03 = m03;

            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;

            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;

            this.m30 = m30;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        // 下标运算符
        public float this[int r, int c]
        {
            get
            {
                if (r == 0 && c == 0)
                    return m00;
                else if (r == 0 && c == 1)
                    return m01;
                else if (r == 0 && c == 2)
                    return m02;
                else if (r == 0 && c == 3)
                    return m03;
                else if (r == 1 && c == 0)
                    return m10;
                else if (r == 1 && c == 1)
                    return m11;
                else if (r == 1 && c == 2)
                    return m12;
                else if (r == 1 && c == 3)
                    return m13;
                else if (r == 2 && c == 0)
                    return m20;
                else if (r == 2 && c == 1)
                    return m21;
                else if (r == 2 && c == 2)
                    return m22;
                else if (r == 2 && c == 3)
                    return m23;
                else if (r == 3 && c == 0)
                    return m30;
                else if (r == 3 && c == 1)
                    return m31;
                else if (r == 3 && c == 2)
                    return m32;
                else if (r == 3 && c == 3)
                    return m33;
                return 0;
            }
            set {                
                if (r == 0 && c == 0)
                    m00 =value;
                else if (r == 0 && c == 1)
                    m01 =value;
                else if (r == 0 && c == 2)
                    m02 =value;
                else if (r == 0 && c == 3)
                    m03 =value;
                else if (r == 1 && c == 0)
                    m10 =value;
                else if (r == 1 && c == 1)
                    m11 =value;
                else if (r == 1 && c == 2)
                    m12 =value;
                else if (r == 1 && c == 3)
                    m13 =value;
                else if (r == 2 && c == 0)
                    m20 =value;
                else if (r == 2 && c == 1)
                    m21 =value;
                else if (r == 2 && c == 2)
                    m22 =value;
                else if (r == 2 && c == 3)
                    m23 =value;
                else if (r == 3 && c == 0)
                    m30 =value;
                else if (r == 3 && c == 1)
                    m31 =value;
                else if (r == 3 && c == 2)
                    m32 =value;
                else if (r == 3 && c == 3)
                    m33 =value;
            }
        }

        // 计算行列式
        public float Determinant()
        {
            float det = 0f;
            for (int i = 0; i < mNumber; i++)
            {
                float remainValue = this._GetRemianValue(0, i);
                det = det + this[0,i] * remainValue;
            }
            return det;
        }
        
        // 获取代数余子式
        public float _GetRemianValue(int trimRow, int trimCol)
        {
            int remainNumber = mNumber - 1;
            // 余子式
            Matrix33 remain = new Matrix33();
            int index = 0;
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    if (row != trimRow && col != trimCol)
                    {
                        int rowIndex = index / remainNumber;
                        int colIndex = index % remainNumber;
                        ++index;
                        remain[rowIndex, colIndex] = this[row, col];
                    }
                }
            }
            // 代数余子式
            float remainValue = remain.Determinant() * (float)Math.Pow(-1, trimRow + trimCol);
            return remainValue;
        }

        // 获取伴随矩阵
        public Matrix44 _GetAdj()
        {
            Matrix44 matrix = new Matrix44();
            for (int i = 0; i < mNumber; i++)
            {
                for (int j = 0; j < mNumber; j++)
                {
                    matrix[j, i] = this._GetRemianValue(i, j);
                }
            }
            return matrix;
        }

        // 获取逆矩阵
        public Matrix44 Inverse()
        {
            float det = this.Determinant();
            Matrix44 adj = this._GetAdj();
            return adj * (1f / det);
        }
        
        public static Matrix44 operator *(Matrix44 lhs, float k)
        {
            Matrix44 matrix = new Matrix44();
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    matrix[row, col] = lhs[row, col] * k;
                }
            }
            return matrix;
        }
        
        public static Vector3 operator *(Vector3 p, Matrix44 m)
        {
            float x = p.x * m[0, 0] + p.y * m[1, 0] + p.z * m[2, 0] + m[3, 0];
            float y = p.x * m[0, 1] + p.y * m[1, 1] + p.z * m[2, 1] + m[3, 1];
            float z = p.x * m[0, 2] + p.y * m[1, 2] + p.z * m[2, 2] + m[3, 2];
            return new Vector3(x, y, z);
        }
        
        public static Matrix44 operator *(Matrix44 m1, Matrix44 m2)
        {
            Matrix44 matrix = new Matrix44();
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    float value = 0;
                    for (int i = 0; i < mNumber; i++)
                    {
                        value = value + m1[row, i] * m2[i, col];
                    }
                    matrix[row, col] = value;
                }
            }
            return matrix;
        }
        
        // 用旋转轴和角度构造矩阵
        public static Matrix44 AngleAxis(float angle, Vector3 axis)
        {
            float rad = angle * (float)Math.PI / 180f;
            float S = (float)Math.Sin(rad);
            float C = (float)Math.Cos(rad);
            float C1  = 1 - C;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            Matrix44 matrix = new Matrix44();
            matrix[0, 0] = x * x * C1 + C;
            matrix[0, 1] = x * y * C1 + z * S;
            matrix[0, 2] = x * z * C1 - y * S;
            matrix[0, 3] = 0;

            matrix[1, 0] = x * y * C1 - z * S;
            matrix[1, 1] = y * y * C1 + C;
            matrix[1, 2] = y * z * C1 + x * S;
            matrix[1, 3] = 0;

            matrix[2, 0] = x * z * C1 + y * S;
            matrix[2, 1] = y * z * C1 - x * S;
            matrix[2, 2] = z * z * C1 + C;
            matrix[2, 3] = 0;
            
            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;
            return matrix;
        }
        
        // 绕axis旋转angle度，然后再平移translate
        public static Matrix44 AngleAxisTranslate(float angle, Vector3 axis, Vector3 translate)
        {
            Matrix44 matrix = AngleAxis(angle, axis);
            matrix[3, 0] = translate.x;
            matrix[3, 1] = translate.y;
            matrix[3, 2] = translate.z;
            matrix[3, 3] = 1;                                         
            return matrix;
        }
        
        // 移动矩阵
        public static Matrix44 Translate(Vector3 translate)
        {
            Matrix44 matrix = new Matrix44(1, 0, 0, 0,   0, 1, 0, 0,   0, 0, 1, 0,   translate.x, translate.y, translate.z, 1);
            return matrix;
        }
    }
}