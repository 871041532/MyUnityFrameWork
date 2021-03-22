using System;
using UnityEditor;
using UnityEngine;

namespace EditorLearn.NodeWindow
{
    // 输入节点
    public enum InputType
    {
        Number, // 数字
        RandomNumber, // 随机数
    }

    public class InputNode : BaseNode
    {
        private InputType m_inputType = InputType.Number;
        private string m_inputVale = "";

        // 获取随机值
        private string m_randomFrom;
        private string m_randomTo;

        // 结果数字
        private string m_resultNumber;

        public InputNode()
        {
            m_title = "InputNode";
        }

        // 获取得到的数字
        public string GetResult()
        {
            return m_resultNumber;
        }

        protected override void OnDrawWindow()
        {
            m_inputType = (InputType) EditorGUILayout.EnumPopup("Input Type", m_inputType);
            if (m_inputType == InputType.Number)
            {
                m_inputVale = EditorGUILayout.TextField("Value", m_inputVale);
                m_resultNumber = m_inputVale;
            }
            else if (m_inputType == InputType.RandomNumber)
            {
                m_randomFrom = EditorGUILayout.TextField("From", m_randomFrom);
                m_randomTo = EditorGUILayout.TextField("From", m_randomTo);
                if (GUILayout.Button("Create Random NUmber"))
                {
                    this.CreateRandomNumber();
                }
            }
        }

        //创建随机数
        private void CreateRandomNumber()
        {
            int from = Convert.ToInt32(m_randomFrom);
            int to = Convert.ToInt32(m_randomTo);
            Debug.Log($"生成的随机数范围{from} {to}");
            m_resultNumber = UnityEngine.Random.Range(from, to).ToString();
            Debug.Log($"生成的随机数结果{m_resultNumber}");
        }
    }
}