using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LuaManager m_LuaMgr;
    public ABManager m_ABMgr;
    public CutSceneManager m_CutSceneMgr;
    public ResourceManager m_ResMgr;
    public ObjectManager m_ObjectMgr;
    public UIManager m_UIMgr;
    public CallManager m_CallMgr;
    public HotPatchManager m_HotPatchMgr;
    private List<IManager> m_Mgrs;

    private void Awake()
    {
        Console.Init();
        Instance = this;
        Assert.raiseExceptions = true;
        DontDestroyOnLoad(gameObject);
        m_Mgrs = new List<IManager>();
        m_CallMgr = new CallManager();
        m_ObjectMgr = new ObjectManager();
        m_ABMgr = new ABManager();
        m_HotPatchMgr = new HotPatchManager();
        m_LuaMgr = new LuaManager();
        m_ResMgr = new ResourceManager();
        m_CutSceneMgr = new CutSceneManager();
        m_UIMgr = new UIManager();
        m_Mgrs.Add(m_CallMgr);
        m_Mgrs.Add(m_LuaMgr);
        m_Mgrs.Add(m_ABMgr);
        m_Mgrs.Add(m_CutSceneMgr);
        m_Mgrs.Add(m_ResMgr);
        m_Mgrs.Add(m_ObjectMgr);
        m_Mgrs.Add(m_UIMgr);
        m_Mgrs.Add(m_HotPatchMgr);

        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Awake();
        }
    }

    private void Start()
    {
        m_CallMgr.RegisterEvent(EventEnum.OnPatched, OnPatched);
        m_CallMgr.RegisterEvent(EventEnum.OnPatchedFail, OnPatchedFailed);
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Start();
        }
    }

    private void OnPatched(params object[] args)
    {
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.OnPatched();
        }
    }
    
    private void OnPatchedFailed(params object[] args)
    {
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.OnPatchedFailed();
        }
    }
    
    private void Update()
    {
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Update();
        }
    }

    private void OnDestroy()
    {
        m_CallMgr.TriggerEvent(EventEnum.OnDestroy);
        m_CallMgr.RemoveEvent(EventEnum.OnPatched, OnPatched);
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.OnDestroy();
        }
        Console.OnDestroy();
    }

    public void OnGUI()
    {
    }
}

public class IManager
{
    public GameManager GameMgr { get { return GameManager.Instance; } }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void OnPatched() {}  // Start之后Patch之前

    public virtual void OnPatchedFailed()
    {
        
    }

    public virtual void Update() { }

    public virtual void OnDestroy() { }
}

public enum LogMode { All, JustErrors };
public enum LogType { Info, Warning, Error };