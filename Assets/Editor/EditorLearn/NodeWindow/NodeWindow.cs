
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorLearn.NodeWindow
{
    public enum MenuType //菜单类型
    {
        Input,
        Output,
        Cale,
        Comp,
        Delete,
        Line
    }
    
    public class NodeWindow : EditorWindow
    {
        // 存放窗口容器
        private List<BaseNode> m_nodes = new List<BaseNode>();

        // 是否点击在窗口上
        private bool m_isClickedOnNode = false;

        // 当前选中窗口的下标
        private int m_selectedIndex = -1;

        // 当前鼠标位置
        private Vector2 m_mousePos;

        // 当前是否为画线模式
        private bool m_isDrawLineModel = false;

        // 当前选中的Node
        private BaseNode m_selectNode;
        
        // 画线模式选中的节点
        private BaseNode m_drawModeSelectedNode;

        // 开始绘制
        private void OnGUI()
        {
            Event eventItem = Event.current;
            m_mousePos = eventItem.mousePosition;
            // 按下鼠标右键操作
            if (eventItem.button == 1 && eventItem.isMouse && !m_isDrawLineModel)
            {
                CreateMenu(eventItem);   
            }
            // 在画线状态下点击鼠标左键
            else if (eventItem.button == 0 && eventItem.isMouse && m_isDrawLineModel)
            {
                FoundSelectedWindow();
                m_drawModeSelectedNode = m_nodes[m_selectedIndex];
                if (m_isClickedOnNode && m_drawModeSelectedNode != null)
                {
                    //1.否则，将输入结点的引用给输出结点
                    m_drawModeSelectedNode.SetInput((InputNode)m_selectNode, m_mousePos);
                    //isDrawLineModel = false;
                    //selectNode = null;
                    //2.将线给连上
                }
            }
            
            //画线功能
            if (m_isDrawLineModel && m_selectNode != null)
            {
                //2.找到结束的位置（矩形）
                Rect endRect = new Rect(m_mousePos, new Vector2(10, 10));
                DrawBezier(m_selectNode.rect, endRect);
                Repaint();
            }
            
            //维护画线功能
            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].DrawBezier();
            }
            
            BeginWindows();  // 开始绘制弹出窗口
            
            for (int i = 0; i < m_nodes.Count; i++)
            {
                var currentNode = m_nodes[i];
                currentNode.rect = GUI.Window(i, currentNode.rect, (winId) =>
                {
                    currentNode.DrawWindow(); 
                    GUI.DragWindow(); //设置窗口可拖动
                }, currentNode.title);
            }
            EndWindows();
        }

        // 创建点击node时的菜单
        private void CreateMenu(Event eventItem)
        {
            FoundSelectedWindow(); //尝试寻找点击的窗体
            if (m_isClickedOnNode)
            {
                this.CreateNodeMenu();  
                eventItem.Use();
                m_isClickedOnNode = false;
                m_selectNode = null;
            }
            else
            {
                this.CreateBlankMenu();
                eventItem.Use();
            }
        }
        
        // 创建点击node的菜单，可以删除窗口和画线
        private void CreateNodeMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Node"), false, MenuCallback, MenuType.Delete);
            menu.AddItem(new GUIContent("Draw Line"), false, MenuCallback, MenuType.Line);
            menu.ShowAsContext();
        }
        
        
        // 创建点击空白处的菜单，可以创建节点
        private void CreateBlankMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Input"), false, MenuCallback, MenuType.Input);
//            menu.AddItem(new GUIContent("Add Output"), false, MenuCallback, MenuType.Output);
//            menu.AddItem(new GUIContent("Add Cale"), true, MenuCallback, MenuType.Cale);
//            menu.AddItem(new GUIContent("Add Comp"), false, MenuCallback, MenuType.Comp);
            menu.ShowAsContext();
        }
        
        
        private void MenuCallback (object type)
        {
            Debug.Log("Enter!!!" + ((MenuType)type).ToString());
            switch ((MenuType)type)
            {
                //在鼠标位置创建指定大小的小窗口
                case MenuType.Input:
                    InputNode input = new InputNode();
                    input.rect = new Rect(m_mousePos.x, m_mousePos.y,200,150);
                    m_nodes.Add(input);
                    break;
                case MenuType.Delete:
                    //删除对应的子窗口
                    for (int i = 0; i < m_nodes.Count; i++)
                    {
                        m_nodes[i].DeleteNode(m_nodes[m_selectedIndex]);
                    }
 
                    m_nodes.RemoveAt(m_selectedIndex);
                    break;
                case MenuType.Line:
                    //写我们的画线逻辑
                    FoundSelectedWindow();
                    //1.找到开始的位置（矩形）
                    m_selectNode = m_nodes[m_selectedIndex];
                    //2.切换当前模式为画线模式
                    m_isDrawLineModel = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        // 发现选择的窗体
        private void FoundSelectedWindow()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                var node = m_nodes[i];
                if (node.rect.Contains(m_mousePos))
                {
                    m_isClickedOnNode = true;
                    m_selectedIndex = i;
                    m_selectNode = node;
                    break;
                }
                else
                {
                    m_isClickedOnNode = false;
                    m_selectedIndex = -1;
                }
            }
        }

        // 绘制贝塞尔曲线
        public static void DrawBezier(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.max.x, start.max.y, 0);
            Vector3 endPos = new Vector3(end.max.x, end.max.y, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadow = new Color(0, 0, 0, 0.7f);
            for (int i = 0; i < 5; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, 1 + (i * 2));
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.grey, null, 1);
        }
    }
}