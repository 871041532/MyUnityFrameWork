using UnityEditor;
using UnityEngine;

namespace EditorLearn.NodeWindow
{
    // 节点编辑器：节点类
    public class BaseNode
    {
        protected string m_title;

        public string title => m_title;

        protected Rect m_rect;

        public Rect rect
        {
            get { return m_rect; }
            set { m_rect = value; }
        }


        // 绘制窗口
        public void DrawWindow()
        {
            m_title = EditorGUILayout.TextField("Title", m_title);
            this.OnDrawWindow();
        }

        public void DeleteNode(BaseNode node)
        {
             this.OnDeleteNode(node);
        }

        public void SetInput(BaseNode inputNode, Vector2 mousePos)
        {
           this.OnSetInput(inputNode, mousePos); 
        }

        public void DrawBezier()
        {
            this.OnDrawBezier();
        }
        

        /****************************************模板方法************************************************/
        // 绘制窗口
        protected virtual void OnDrawWindow()
        {
        }

        // 设置输入
        protected virtual void OnSetInput(BaseNode inputNode, Vector2 mousePos)
        {
        }

        // 绘制贝塞尔曲线
        protected virtual void OnDrawBezier()
        {
        }

        // 删除节点
        protected virtual void OnDeleteNode(BaseNode node)
        {
        }
    }
}