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
        DontDestroyOnLoad(gameObject);
        m_Mgrs = new List<IManager>();

        m_CallMgr = new CallManager();
        m_ObjectMgr = new ObjectManager();
        m_LuaMgr = new LuaManager();

        m_ABMgr = new ABManager();
        m_ResMgr = new ResourceManager();
        m_CutSceneMgr = new CutSceneManager();
        m_UIMgr = new UIManager();
        m_HotPatchMgr = new HotPatchManager();


        m_Mgrs.Add(m_LuaMgr);
        m_Mgrs.Add(m_ABMgr);
        m_Mgrs.Add(m_CutSceneMgr);
        m_Mgrs.Add(m_ResMgr);
        m_Mgrs.Add(m_ObjectMgr);
        m_Mgrs.Add(m_UIMgr);
        m_Mgrs.Add(m_CallMgr);
        m_Mgrs.Add(m_HotPatchMgr);
        
        Assert.raiseExceptions = true;
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Awake();
        }
    }

    private void Start()
    {
        var iter = m_Mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Start();
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
    public virtual void Update() { }
    public virtual void OnDestroy() { }
}

public enum LogMode { All, JustErrors };
public enum LogType { Info, Warning, Error };