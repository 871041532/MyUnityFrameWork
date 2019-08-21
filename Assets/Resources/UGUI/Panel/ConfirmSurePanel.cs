using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmSurePanel : Window
{
    public ConfirmSurePanel(string prefabPath = "Assets/GameData/UI/prefabs/ConfirmSure.prefab") : base(prefabPath) { }

    public Button m_Button;
    public Text m_Text;
    private Action m_ConfirmCall; 

    protected override void OnInit()
    {
        m_Button = m_TransForm.Find("Button").GetComponent<Button>();
        m_Text = m_TransForm.Find("Text").GetComponent<Text>();
    }

    void ConfirmCall()
    {
        m_ConfirmCall?.Invoke();
    }
    
    protected override void OnShow(params object[] args)
    {
        string text = args[0] as string;
        m_Text.text = text;
        m_ConfirmCall = args[1] as Action;
        m_GameObject.transform.SetAsLastSibling();
        m_Button.onClick.AddListener(ConfirmCall);
    }

    protected override void OnHide()
    {
        m_Button.onClick.RemoveListener(ConfirmCall);
        m_ConfirmCall = null;
    }
    
}