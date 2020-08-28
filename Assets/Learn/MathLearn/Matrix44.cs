using System;
using UnityEngine.SocialPlatforms;

namespace MathLearn
{
    public struct Matrix44
    {
        private float[,] myCells;
        private static int mNumber = 4;

        // 用离散的float数造矩阵
        public Matrix44(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            myCells = new float[,]{
                {m11, m12, m13, m14},
                {m21, m22, m23, m24},
                {m31, m32, m33, m34},
                {m41, m42, m43, m44},
            }; 
        }

        // 用数组构造矩阵
        public Matrix44(float[,] cells = null)
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
            Matrix33 remain = new Matrix33(cells[0,0], cells[0,1], cells[0,2], cells[1,0], cells[1,1], cells[1,2], cells[2,0], cells[2,1], cells[2,2]);
            // 代数余子式
            float remainValue = remain.Determinant() * (float)Math.Pow(-1, trimRow + trimCol);
            return remainValue;
        }

        // 获取伴随矩阵
        public Matrix44 _GetAdj()
        {
            float[,] cells = new float[mNumber, mNumber];
            for (int i = 0; i < mNumber; i++)
            {
                for (int j = 0; j < mNumber; j++)
                {
                    cells[j, i] = this._GetRemianValue(i, j);
                }
            }
            return new Matrix44(cells);
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
            float[,] cells = new float[mNumber, mNumber];
            for (int row = 0; row < mNumber; row++)
            {
                for (int col = 0; col < mNumber; col++)
                {
                    cells[row, col] = lhs[row, col] * k;
                }
            }
            return new Matrix44(cells);
        }
        
        public static Vector3 operator *(Vector3 p, Matrix44 m)
        {
            var cells = m.myCells;
            float x = p.x * cells[0, 0] + p.y * cells[1, 0] + p.z * cells[2, 0] + cells[3, 0];
            float y = p.x * cells[0, 1] + p.y * cells[1, 1] + p.z * cells[2, 1] + cells[3, 1];
            float z = p.x * cells[0, 2] + p.y * cells[1, 2] + p.z * cells[2, 2] + cells[3, 2];
            return new Vector3(x, y, z);
        }
        
        public static Matrix44 operator *(Matrix44 m1, Matrix44 m2)
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
            return new Matrix44(cells);
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
            float[,] cells = new float[mNumber, mNumber];
            cells[0, 0] = x * x * C1 + C;
            cells[0, 1] = x * y * C1 + z * S;
            cells[0, 2] = x * z * C1 - y * S;
            cells[0, 3] = 0;

            cells[1, 0] = x * y * C1 - z * S;
            cells[1, 1] = y * y * C1 + C;
            cells[1, 2] = y * z * C1 + x * S;
            cells[1, 3] = 0;

            cells[2, 0] = x * z * C1 + y * S;
            cells[2, 1] = y * z * C1 - x * S;
            cells[2, 2] = z * z * C1 + C;
            cells[2, 3] = 0;
            
            cells[3, 0] = 0;
            cells[3, 1] = 0;
            cells[3, 2] = 0;
            cells[3, 3] = 1;
            return new Matrix44(cells);
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