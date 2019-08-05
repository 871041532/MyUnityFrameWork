using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Window
{
    protected string m_PrefabPath;
    public string PrefabPath { get { return m_PrefabPath; } }
    private string m_Name = "DefaultName";
    public string Name { get { return m_Name; } }
    public GameObject m_GameObject;
    public RectTransform m_TransForm;
    private bool m_IsVisible;
    public bool IsVisible { get { return m_IsVisible; } }
    private bool m_HavenInit = false;
    private bool m_HavenDestroy = false;

    public Action m_OnInit = null;
    public Action<object[]> m_OnShow = null;
    public Action m_OnHide = null;
    public Action m_OnUpdate = null;
    public Action m_OnDestroy = null;

    public Window(string prefabPath = null)
    {
        if (!string.IsNullOrEmpty(prefabPath))
        {
            m_PrefabPath = prefabPath;
        }
    }

    public void SetGameObjectAndName(GameObject obj, string name)
    {
        m_Name = name;
        m_GameObject = obj;
        m_TransForm = obj.transform as RectTransform;
    }

    // 只有UIM才能调的方法
    public void DestroyCallByUIMgr()
    {
        m_GameObject = null;
        m_TransForm = null;
        m_PrefabPath = null;
        OnDestroy();
        m_OnDestroy?.Invoke();
    }

    private void Init()
    {
        if (!m_HavenInit)
        {
            m_HavenInit = true;
            OnInit();
            m_OnInit?.Invoke();
        }
    }

    #region 通用方法
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
        Init();
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
        if (!m_HavenDestroy)
        {
            m_HavenDestroy = true;
            GameManager.Instance.m_UIMgr.DestroyWindow(this);
        }
        else
        {
            Debug.LogError("不可重复回收Window对象！");
        }
    }
    #endregion

    #region 模板方法
    protected virtual void OnInit() { }
    protected virtual void OnShow(params object[] args) { }
    protected virtual void OnHide() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDestroy() { }
    #endregion
}