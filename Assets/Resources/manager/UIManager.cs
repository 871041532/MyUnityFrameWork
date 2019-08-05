using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : IManager
{
    // UI根节点
    private RectTransform m_UIRoot;
    // winRoot
    private RectTransform m_WinRoot;
    // UICamera
    private Camera m_UICamera;
    // EventSystem
    private EventSystem m_EventSystem;
    // 分辨率
    private Vector2 m_ReferenceResolution;
    // 注册字典
    public Dictionary<string, Func<Window>> m_RegisterDic = new Dictionary<string, Func<Window>>();
    // 打开的窗口
    private Dictionary<string, Window> m_WinDic = new Dictionary<string, Window>();
    // 对象池
    private CoreCompositePool m_Pool = new CoreCompositePool();
    private List<Window> m_TempList = new List<Window>();

    public override void Awake()
    {
        m_UIRoot = GameMgr.gameObject.transform.Find("UIRoot") as RectTransform;
        m_WinRoot = m_UIRoot.Find("WinRoot") as RectTransform;
        m_UICamera = m_UIRoot.Find("UICamera").GetComponent<Camera>();
        m_EventSystem = GameMgr.gameObject.transform.Find("EventSystem").GetComponent<EventSystem>();
        var canvasScaler = m_UIRoot.GetComponent<CanvasScaler>();
        m_ReferenceResolution = canvasScaler.referenceResolution;
        RegisterWindow("menu", () =>
        {
            return new MenuPanel();
        });
        RegisterWindow("loading", () =>
        {
            return new LoadingPanel();
        });
        var win = GetOrCreateWindow("menu");
        win.Show();
    }

    public override void Update()
    {
        foreach (var item in m_WinDic)
        {
            item.Value.Update();
        }
    }

    // 注册 name -> window
    private void RegisterWindow(System.Type type)
    {
        RegisterWindow(type.Name, () => {
            return Activator.CreateInstance(type) as Window;
        });
    }

    public void RegisterWindow(string windowName, Func<Window> createAction)
    {
        m_RegisterDic[windowName] = createAction;
    }

    public void UnRegisterWindow(string windowName)
    {
        m_RegisterDic.Remove(windowName);
    }

    /// <summary>
    /// 查找打开的Window
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Window FindWindowByName(string name)
    {
        Window win = null;
        m_WinDic.TryGetValue(name, out win);
        return win;
    }

    /// <summary>
    /// 打开一个window
    /// </summary>
    /// <param name="windowName"></param>
    /// <returns></returns>
    public Window GetOrCreateWindow(string windowName)
    {
        Window win = FindWindowByName(windowName);
        if (win is null)
        {
            Func<Window> createCall= null;
            m_RegisterDic.TryGetValue(windowName, out createCall);
            if (!(createCall is null))
            {
                win = createCall();
            }
            else
            {
                Debug.LogError(windowName + "窗口名对应的Window创建函数未注册！");
            }
            GameObject obj = m_Pool.Spawn(win.PrefabPath, m_WinRoot);
            obj.transform.SetAsLastSibling();
            win.SetGameObjectAndName(obj, windowName);
            if (!m_WinDic.ContainsKey(windowName))
            {
                m_WinDic.Add(windowName, win);
            }
        }
        return win;
    }

    /// <summary>
    /// 展示一个window
    /// </summary>
    /// <param name="win"></param>
    /// <param name="args"></param>
    public void ShowWindow(Window win, params object[] args)
    {
        win.Show(args);
    }

    /// <summary>
    /// 隐藏一个window
    /// </summary>
    /// <param name="win"></param>
    public void HideWindow(Window win)
    {
        win.Hide();
    }

    public void HideAllWindow()
    {
        foreach (var item in m_WinDic)
        {
            HideWindow(item.Value);
        }
    }

    /// <summary>
    /// destroy一个window
    /// </summary>
    /// <param name="win"></param>
    /// <param name="destroy"></param>
    public void  DestroyWindow(Window win)
    {
        win.Hide();
        m_Pool.Recycle(win.PrefabPath, win.m_GameObject);
        m_Pool.DestroyOne(win.PrefabPath, true);
        win.DestroyCallByUIMgr();
        m_WinDic.Remove(win.Name);
    }

    /// <summary>
    /// 关闭所有window
    /// </summary>
    public void DestroyAllWindow()
    {
        m_TempList.Clear();
        m_TempList.AddRange(m_WinDic.Values);
        for (int i = 0; i < m_TempList.Count; i++)
        {
            DestroyWindow(m_TempList[i]);
        }
    }

    /// <summary>
    /// 清除所有隐藏的window
    /// </summary>
    public void DestroyHideWindow()
    {
        m_TempList.Clear();
        m_TempList.AddRange(m_WinDic.Values);
        for (int i = 0; i < m_TempList.Count; i++)
        {
            var win = m_TempList[i];
            if (!win.IsVisible)
            {
                DestroyWindow(m_TempList[i]);
            }
        }
    }

    /// <summary>
    /// 切换到一个单窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="args"></param>
    public Window SwitchSingleWindow(string name, params object[] args)
    {
        HideAllWindow();
        Window win = GetOrCreateWindow(name);
        win.Show(args);
        return win;
    }

}
