using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1、首先new一个添加点用来绘制；
// 2、一个创建材质的函数，创建绘制需要的材质；
// 3、在 OnRenderObject() 函数绘制线段和平面；
// 4、分别创建画线和绘制平面
public class GLPaint : MonoBehaviour
{
    private List<Vector3> m_tempList;  // 存放点的列表
    void Start()
    {
        m_tempList = new  List<Vector3>();
        m_tempList.Add(new Vector3(0, 0, 0));
        m_tempList.Add(new Vector3(0, 3, 0));
        m_tempList.Add(new Vector3(3, 3, 0));
        m_tempList.Add(new Vector3(3, 0, 0));
        CreateLineMaterial();
    }

    public Material lineMaterial;

    void CreateLineMaterial()
    {
        if (lineMaterial == null)
        {
            Shader shader = Shader.Find("MathLearn/GLPaintShader");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
//            lineMaterial.SetColor("m_Color", Color.yellow);
            
        }
    }

    private void OnRenderObject()
    {
        lineMaterial.SetPass(0);
        DrawTriangle(m_tempList);
    }

    /// GL画线函数
    void DrawLines(List<Vector3> tmpBorderPoints)
    {
        GL.PushMatrix();
        // Draw lines
        GL.Begin(GL.LINES);
        // Vertex colors change from red to green
        GL.Color(Color.red);
        if (tmpBorderPoints != null)
        {
            for (int i = 0; i < tmpBorderPoints.Count; i++)
            {
                Vector3 posOne = tmpBorderPoints[i]; 
                Vector3 posTwo = tmpBorderPoints[(i + 1) % tmpBorderPoints.Count];         
                // One vertex at transform position
                GL.Vertex3(posOne.x, posOne.y, posOne.z);
                // Another vertex at edge of circle
                GL.Vertex3(posTwo.x, posTwo.y, posTwo.z);
            }
        }
        GL.End();
        GL.PopMatrix();
    }

    // GL 平面绘制
    void DrawTriangle(List<Vector3> tmpBorderPoints)
    {
        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);
        // Draw lines
        GL.Begin(GL.TRIANGLES);
        // Vertex colors change from red to green
        GL.Color(Color.green);
        if (tmpBorderPoints != null)
        {
            for (int i = 0; i < tmpBorderPoints.Count; i++)
            {
                if (i < tmpBorderPoints.Count - 2)
                {
                    // 以第一个为原点，绘制三角形面
                    Vector3 posZero = tmpBorderPoints[0];
                    Vector3 posOne = tmpBorderPoints[i + 1];
                    Vector3 posTwo = tmpBorderPoints[(i + 2)];
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(posZero.x, posZero.y, posZero.z);
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(posOne.x, posOne.y, posOne.z);
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(posTwo.x, posTwo.y, posTwo.z);
                }
            }
        }
        GL.End();
        GL.PopMatrix();
    }
}
































