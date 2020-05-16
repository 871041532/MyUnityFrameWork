using System;
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
        m_GameObject.transform.SetAsLastSibling();
        GameManager.Instance.m_CallMgr.RegisterEvent(EventEnum.OnPatchInfoUpdate, UpdateInfo);
        GameManager.Instance.m_CallMgr.RegisterEvent(EventEnum.OnPatchedFail, PatchError);
    }

    protected override void OnHide()
    {
        GameManager.Instance.m_CallMgr.RemoveEvent(EventEnum.OnPatchInfoUpdate, UpdateInfo);
        GameManager.Instance.m_CallMgr.RemoveEvent(EventEnum.OnPatchedFail, PatchError);
    }

    void UpdateInfo(params object[] args)
    {
        var step = GameManager.Instance.m_HotPatchMgr.CurrentStep;
        var progress = GameManager.Instance.m_HotPatchMgr.CurStepProgress;
        var strs = $"当前状态：{step.ToString()}   进度：{progress*100}%";
        m_Text.text = strs;
        m_Slider.value = progress;
    }

    void PatchError(params object[] args)
    {
        string info = "";
        var patchManager = GameManager.Instance.m_HotPatchMgr;
        info = "Patch错误：" + patchManager.State.ToString();
        
        var win = GameManager.Instance.m_UIMgr.GetOrCreateWindow("confirm");
        Action func = () =>
        {
            Debug.Log("点击了确认按钮！");
            win.Destroy();
           this.Destroy();
//            GameManager.Instance.m_UIMgr.SwitchSingleWindow("menu");
        };
        win.Show(info, func);
    }
}