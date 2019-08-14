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
    public AssetItem m_ClipAsset;

    protected override void OnInit()
    {
        m_ClipAsset = GameManager.Instance.m_ABMgr.LoadAsset("Assets/GameData/Audio/zhong.wav");
        m_BtnLoad = m_TransForm.Find("BtnLoad").GetComponent<Button>();
        m_BtnStart = m_TransForm.Find("BtnStart").GetComponent<Button>();
        m_BtnExit = m_TransForm.Find("BtnExit").GetComponent<Button>();

        m_BtnLoad.onClick.AddListener(()=> {
            GameManager.Instance.m_UIMgr.SwitchSingleWindow("loading", "scene2");
        });

        m_BtnStart.onClick.AddListener(() => {
            GameManager.Instance.m_UIMgr.SwitchSingleWindow("sportTool");
        });

        m_BtnExit.onClick.AddListener(() => {
            Destroy();
        });
    }

    protected override void OnDestroy()
    {
        GameManager.Instance.m_ABMgr.UnloadAsset(m_ClipAsset);
    }
}