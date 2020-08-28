using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix33
    {
        private float[,] myCells;
        private static int mNumber = 3;

        // 用vector构造矩阵 
        public Matrix33(Vector3 line1, Vector3 line2, Vector3 line3)
        {
            myCells = new float[,]{
                {line1.x, line1.y, line1.z},
                {line2.x, line2.y, line2.z},
                {line3.x, line3.y, line3.z},
            };
        }

        // 用离散的float数造矩阵
        public Matrix33(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            myCells = new float[,]{
                {m11, m12, m13},
                {m21, m22, m23},
                {m31, m32, m33},
            }; 
        }

        // 用数组构造矩阵
        public Matrix33(float[,] cells = null)
        {
            if (cells == null)
            {
                myCells = new float[mNumber, mNumber];
            }
            else
            {
                myCells = cells;   
            } 
        }

        // 下标运算符
        public float this[int rowIndex, int colIndex]
        {
            get { return myCells[rowIndex, colIndex]; }
            set { myCells[rowIndex, colIndex] = value; }
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