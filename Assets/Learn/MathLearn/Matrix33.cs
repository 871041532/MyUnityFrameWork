using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix33
    {
        private float[,] myCells;

        public Matrix33(Vector3 line1, Vector3 line2, Vector3 line3)
        {
            myCells = new float[,]{
                {line1.x, line1.y, line1.z},
                {line2.x, line2.y, line2.z},
                {line3.x, line3.y, line3.z},
            };
        }

        public Matrix33(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            myCells = new float[,]{
                {m11, m12, m13},
                {m21, m22, m23},
                {m31, m32, m33},
            }; 
        }

        // 计算行列式
        public float Determinant()
        {
            float det = 0f;
            for (int i = 0; i < 3; i++)
            {
                Matrix22 remain = _GetRemian(0, i);
                det = det + myCells[0,i] * remain.Determinant() * (float)Math.Pow(-1, i);
            }
            return det;
        }
        
        // 获取余子式
        public Matrix22 _GetRemian(int trimRow, int trimCol)
        {
            float[,] cells = new float[2, 2];
            int index = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (row != trimRow && col != trimCol)
                    {
                        int rowIndex = index / 2;
                        int colIndex = index % 2;
                        ++index;
                        cells[rowIndex, colIndex] = myCells[row, col];
                    }
                }
            }
            return new Matrix22(cells[0, 0], cells[0,1], cells[1, 0], cells[1, 1]);
        }

        // 获取伴随矩阵
        public Matrix33 _GetAdj()
        {
            float[,] cells = new float[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Matrix22 remain = _GetRemian(i, j);
                    float k = (float) Math.Pow(-1, i + j);
                    cells[j, i] = remain.Determinant() * k;
                }
            }
            return new Matrix33(cells[0, 0], cells[0, 1], cells[0, 2], cells[1, 0], cells[1, 1], cells[1, 2],
                cells[2, 0], cells[2, 1], cells[2, 2]);
        }

        // 获取逆矩阵
        public Matrix33 Inverse()
        {
            float det = this.Determinant();
            Matrix33 adj = this._GetAdj();
            return adj / det;
        }
        
        public static Matrix33 operator /(Matrix33 lhs, float k)
        {
            Vector3 line1 = new Vector3(lhs.myCells[0,0] * k, lhs.myCells[0,1] * k, lhs.myCells[0,2] * k);
            Vector3 line2 = new Vector3(lhs.myCells[1,0] * k, lhs.myCells[1,1] * k, lhs.myCells[1,2] * k);
            Vector3 line3 = new Vector3(lhs.myCells[2,0] * k, lhs.myCells[2,1] * k, lhs.myCells[2,2] * k);
            return new Matrix33(line1, line2, line3);
        }
        
        public static Vector3 operator *(Vector3 p, Matrix33 m)
        {
            var cells = m.myCells;
            float x = p.x * cells[0, 0] + p.y * cells[1, 0] + p.z * cells[2, 0];
            float y = p.x * cells[0, 1] + p.y * cells[1, 1] + p.z * cells[2, 1];
            float z = p.x * cells[0, 2] + p.y * cells[1, 2] + p.z * cells[2, 2];
            return new Vector3(x, y, z);
        }
//
//        public static Matrix33 operator *(Matrix33 lhs, Matrix33 rhs)
//        {
//            float t11 = lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
//            float t12 = lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
//            float t21 = lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
//            float t22 = lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
//            return new Matrix33(t11, t12, t21, t22);
//        }
//

//
//        public static Matrix33 Rotation(float angle)
//        {
//            float rad = angle * (float)Math.PI / 180;
//            float cosValue = (float)Math.Cos(rad);
//            float sinValue = (float)Math.Sin(rad);
//            return new Matrix33(cosValue, sinValue, -sinValue, cosValue);
//        }
    }
}