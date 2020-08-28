using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix33
    {
        private float[,] myCells;
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
            myCells = null;
        }

        // 用离散的float数造矩阵
        public Matrix33(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            myCells = null;
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
                {
                    return m00;
                }
            }
            set {; }
        }

        // 计算行列式
        public float Determinant()
        {
            float det = 0f;
            for (int i = 0; i < mNumber; i++)
            {
                float remainValue = this._GetRemianValue(0, i);
                det = det + myCells[0,i] * remainValue;
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
                        cells[rowIndex, colIndex] = myCells[row, col];
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
            float[,] cells = new float[mNumber, mNumber];
            for (int i = 0; i < mNumber; i++)
            {
                for (int j = 0; j < mNumber; j++)
                {
                    cells[j, i] = this._GetRemianValue(i, j);
                }
            }
            return new Matrix33(cells);
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
            float[,] cells = new float[mNumber, mNumber];
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    cells[row, col] = lhs[row, col] * k;
                }
            }
            return new Matrix33(cells);
        }
        
        public static Vector3 operator *(Vector3 p, Matrix33 m)
        {
            var cells = m.myCells;
            float x = p.x * cells[0, 0] + p.y * cells[1, 0] + p.z * cells[2, 0];
            float y = p.x * cells[0, 1] + p.y * cells[1, 1] + p.z * cells[2, 1];
            float z = p.x * cells[0, 2] + p.y * cells[1, 2] + p.z * cells[2, 2];
            return new Vector3(x, y, z);
        }
        
        public static Matrix33 operator *(Matrix33 m1, Matrix33 m2)
        {
            var cells = new float[mNumber, mNumber];
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    float value = 0;
                    for (int i = 0; i < mNumber; i++)
                    {
                        value = value + m1[row, i] * m2[i, col];
                    }
                    cells[row, col] = value;
                }
            }
            return new Matrix33(cells);
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
            float[,] cells = new float[mNumber, mNumber];
            cells[0, 0] = x * x * C1 + C;
            cells[0, 1] = x * y * C1 + z * S;
            cells[0, 2] = x * z * C1 - y * S;

            cells[1, 0] = x * y * C1 - z * S;
            cells[1, 1] = y * y * C1 + C;
            cells[1, 2] = y * z * C1 + x * S;

            cells[2, 0] = x * z * C1 + y * S;
            cells[2, 1] = y * z * C1 - x * S;
            cells[2, 2] = z * z * C1 + C;
            return new Matrix33(cells);
        }
    }
}