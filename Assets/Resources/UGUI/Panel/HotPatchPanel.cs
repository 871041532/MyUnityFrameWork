using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HotPatchPanel : Window
{
    public HotPatchPanel(string prefabPath = "Assets/GameData/UI/prefabs/HotPatchPanel.prefab") : base(prefabPath) { }

    public Slider m_Slider;
    public Text m_Text;

    protected override void OnInit()
    {
        m_Slider = m_TransForm.Find("Slider").GetComponent<Slider>();
        m_Text = m_TransForm.Find("Text").GetComponent<Text>();
        m_Slider.maxValue = 1;
        m_Slider.minValue = 0;
    }

    protected override void OnShow(params object[] args)
    {
    }

    protected override void OnDestroy()
    {
    }
}