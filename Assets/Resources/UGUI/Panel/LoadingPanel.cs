using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingPanel : Window
{
    public LoadingPanel(string prefabPath = "Assets/GameData/UI/prefabs/LoadingPanel.prefab") : base(prefabPath) { }

    public Slider m_Slider;

    protected override void OnInit()
    {
        m_Slider = m_TransForm.Find("Slider").GetComponent<Slider>();
        m_Slider.maxValue = 1;
        m_Slider.minValue = 0;
    }

    protected override void OnShow(params object[] args)
    {
        string name = args[0] as string;
        GameManager.Instance.m_CutSceneMgr.BeginLoadScene("Assets/GameData/Scenes/scene2.unity", () => {
            Debug.Log("场景加载成功！");
            GameManager.Instance.m_UIMgr.SwitchSingleWindow("menu");
        }, (i) => {
            Debug.Log("场景加载进度: " + i);
            m_Slider.value = i;
        });
    }

    protected override void OnDestroy()
    {

    }
}