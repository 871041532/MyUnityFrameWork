using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuPanel : Window
{
    public MenuPanel(string prefabPath = "Assets/GameData/UI/prefabs/MenuPanel.prefab") : base(prefabPath) { }

    public Button m_BtnLoad;
    public Button m_BtnStart;
    public Button m_BtnExit;

    protected override void OnInit()
    {
        m_BtnLoad = m_TransForm.Find("BtnLoad").GetComponent<Button>();
        m_BtnStart = m_TransForm.Find("BtnStart").GetComponent<Button>();
        m_BtnExit = m_TransForm.Find("BtnExit").GetComponent<Button>();

        m_BtnLoad.onClick.AddListener(()=> {
            GameManager.Instance.m_UIMgr.SwitchSingleWindow("loading", "scene2");
        });

        m_BtnStart.onClick.AddListener(() => {

        });

        m_BtnExit.onClick.AddListener(() => {

        });
    }

    protected override void OnDestroy()
    {
        
    }
}