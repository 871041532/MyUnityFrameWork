using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SportToolPanel : Window
{
    public SportToolPanel(string prefabPath = "Assets/GameData/UI/prefabs/SportToolPanel.prefab") : base(prefabPath) { }

    public Button m_BtnStart;
    public Button m_BtnStop;
    public Button m_BtnSwitch;
    public Text m_SwitchText;
    public Text m_TipText;
    public AssetItem m_ClipAsset;
    public bool m_IsOpenState = true;
    public float m_LastTime = -1;
    UnityAction m_Restart;
    IEnumerator iter;

    protected override void OnInit()
    {
        m_BtnStart = m_TransForm.Find("BtnStart").GetComponent<Button>();
        m_BtnStop = m_TransForm.Find("BtnStop").GetComponent<Button>();
        m_BtnSwitch = m_TransForm.Find("BtnSwitch").GetComponent<Button>();
        m_TipText = m_TransForm.Find("Tip").GetComponent<Text>();
        m_SwitchText = m_BtnSwitch.transform.Find("Text").GetComponent<Text>();
        m_ClipAsset = GameManager.Instance.m_ABMgr.LoadAsset("Assets/GameData/Audio/zhong.wav");
        m_Restart = () =>
        {
            if (!(iter is null))
            {
                GameManager.Instance.StopCoroutine(iter);
            }
            iter = StartRun();
            GameManager.Instance.StartCoroutine(iter);
        };
        m_BtnStart.onClick.AddListener(m_Restart);

        m_BtnStop.onClick.AddListener(() => {
            if (!(iter is null))
            {
                GameManager.Instance.StopCoroutine(iter);
                iter = null;
                ResetTime();
            }
        });

        m_BtnSwitch.onClick.AddListener(() => {
            m_IsOpenState = !m_IsOpenState;
            RefreshUI();
        });
    }

    IEnumerator StartRun()
    {
        AudioSource.PlayClipAtPoint(m_ClipAsset.AudioClip, Camera.main.transform.position);
        m_LastTime = 30f;
        RefreshUI();
        while (m_LastTime > 0)
        {
            yield return new WaitForSeconds(1);
            m_LastTime -= 1;
            RefreshUI();
        }
        m_Restart();
    }

    void ResetTime()
    {
        m_LastTime = -1;
        RefreshUI();
    }

    void RefreshUI()
    {
        m_SwitchText.text = m_IsOpenState ? "当前公开场合" : "当前内部场合";
        if (m_LastTime < 0)
        {
            m_TipText.text = "说明：点击开始运动休息交替30秒";
        }
        else
        {
            m_TipText.text = string.Format("运动剩余: {0} 秒", m_LastTime);
        }
    }

    protected override void OnDestroy()
    {
        GameManager.Instance.m_ABMgr.UnloadAsset(m_ClipAsset);
    }
}