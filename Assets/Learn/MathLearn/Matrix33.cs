using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix33
    {
        private float m00;
        private float m01;
        private float m02;
        
        private float m10;
        private float m11;
        private float m12;
        
        private float m20;
        private float m21;
        private float m22;
        
        private static int mNumber = 3;

        // 用vector构造矩阵 
        public Matrix33(Vector3 line1, Vector3 line2, Vector3 line3)
        {
            m00 = line1.x;
            m01 = line1.y;
            m02 = line1.z;

            m10 = line2.x;
            m11 = line2.y;
            m12 = line2.z;

            m20 = line3.x;
            m21 = line3.y;
            m22 = line3.z;
        }

        // 用离散的float数造矩阵
        public Matrix33(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            this.m00 = m00;
            this.m01 = m01;
            this.m02 = m02;

            this.m10 = m10;
            this.m11 = m11;
            this.m12 = m12;

            this.m20 = m20;
            this.m21 = m21;
            this.m22 = m22;
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
                else if (r == 1 && c == 0)
                    return m10;
                else if (r == 1 && c == 1)
                    return m11;
                else if (r == 1 && c == 2)
                    return m12;
                else if (r == 2 && c == 0)
                    return m20;
                else if (r == 2 && c == 1)
                    return m21;
                else if (r == 2 && c == 2)
                    return m22;
                return 0;
            }
            set {                
                if (r == 0 && c == 0)
                    m00 = value;
                else if (r == 0 && c == 1)
                    m01 = value;
                else if (r == 0 && c == 2)
                    m02 = value;
                else if (r == 1 && c == 0)
                    m10 = value;
                else if (r == 1 && c == 1)
                    m11 = value;
                else if (r == 1 && c == 2)
                    m12 = value;
                else if (r == 2 && c == 0)
                    m20 = value;
                else if (r == 2 && c == 1)
                    m21 = value;
                else if (r == 2 && c == 2)
                    m22 = value;
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
            float[,] cells = new float[remainNumber, remainNumber];
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
                        cells[rowIndex, colIndex] = this[row, col];
                    }
                }
            }
            // 余子式
            Matrix22 remain = new Matrix22(cells);
            // 代数余子式
            float remainValue = remain.Determinant() * (float)Math.Pow(-1, trimRow + trimCol);
            return remainValue;
        }

        // 获取伴随矩阵
        public Matrix33 _GetAdj()
        {
            Matrix33 matrix = new Matrix33();
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
        public Matrix33 Inverse()
        {
            float det = this.Determinant();
            Matrix33 adj = this._GetAdj();
            return adj * (1f / det);
        }
        
        public static Matrix33 operator *(Matrix33 lhs, float k)
        {
            Matrix33 matrix = new Matrix33();
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    matrix[row, col] = lhs[row, col] * k;
                }
            }
            return matrix;
        }
        
        public static Vector3 operator *(Vector3 p, Matrix33 m)
        {
            float x = p.x * m[0, 0] + p.y * m[1, 0] + p.z * m[2, 0];
            float y = p.x * m[0, 1] + p.y * m[1, 1] + p.z * m[2, 1];
            float z = p.x * m[0, 2] + p.y * m[1, 2] + p.z * m[2, 2];
            return new Vector3(x, y, z);
        }
        
        public static Matrix33 operator *(Matrix33 m1, Matrix33 m2)
        {
            Matrix33 matrix = new Matrix33();
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
        
        // 用角度和旋转轴构造矩阵
        public static Matrix33 AngleAxis(float angle, Vector3 axis)
        {
            float rad = angle * (float)Math.PI / 180f;
            float S = (float)Math.Sin(rad);
            float C = (float)Math.Cos(rad);
            float C1  = 1 - C;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            
            Matrix33 matrix = new Matrix33();
            matrix[0, 0] = x * x * C1 + C;
            matrix[0, 1] = x * y * C1 + z * S;
            matrix[0, 2] = x * z * C1 - y * S;

            matrix[1, 0] = x * y * C1 - z * S;
            matrix[1, 1] = y * y * C1 + C;
            matrix[1, 2] = y * z * C1 + x * S;

            matrix[2, 0] = x * z * C1 + y * S;
            matrix[2, 1] = y * z * C1 - x * S;
            matrix[2, 2] = z * z * C1 + C;
            return matrix;
        }
    }
}