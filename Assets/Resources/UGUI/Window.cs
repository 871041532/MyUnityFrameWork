using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window
{
    public GameObject m_GameObject;
    public RectTransform m_TransForm;
    private string m_PrefabPath;
    public string PrefabPath { get { return m_PrefabPath; } }
    private bool m_IsVisible;
    public bool IsVisible { get { return m_IsVisible; } }

    public Action m_OnInit = null;
    public Action<object[]> m_OnShow = null;
    public Action m_OnHide = null;
    public Action m_OnUpdate = null;
    public Action m_OnDestroy = null;

    public Window()
    {
        m_PrefabPath = GetPrefabPath();
    }

    public void Init(GameObject obj)
    {
        m_GameObject = obj;
        m_TransForm = obj.transform as RectTransform;
        OnInit();
        m_OnInit?.Invoke();

        var btn = m_TransForm.Find("BtnLoad").GetComponent<Button>();
        btn.onClick.AddListener(()=> {
            GameManager.Instance.m_CallMgr.TriggerEvent("login");
        });
    }

    public void SetVisible(bool visible)
    {
        m_IsVisible = visible;
        if (m_GameObject.activeSelf != visible)
        {
            m_GameObject.SetActive(visible);
        }
    }

    public void Show(params object[] args)
    {
        SetVisible(true);
        OnShow(args);
        m_OnShow?.Invoke(args);
    }

    public void Hide()
    {
        SetVisible(false);
        if (m_GameObject.activeSelf)
        {
            m_GameObject.SetActive(false);
        }
        OnHide();
        m_OnHide?.Invoke();
    }

    public void Update()
    {
        OnUpdate();
        m_OnUpdate?.Invoke();
    }

    public void Destroy()
    {
        m_GameObject = null;
        m_TransForm = null;
        m_PrefabPath = null;
        OnDestroy();
        m_OnDestroy?.Invoke();
    }

    protected virtual void OnInit() { }
    protected virtual void OnShow(params object[] args) { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDestroy() { }

    protected virtual string GetPrefabPath()
    {
        return "Assets/GameData/UI/prefabs/MenuPanel.prefab";
    }

}