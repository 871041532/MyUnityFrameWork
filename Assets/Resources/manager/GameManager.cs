using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
    private List<IManager> m_Mgrs;
    public Text m_log_object;

    public void Log(string line)
    {
        string text = m_log_object.text;
        m_log_object.text = text + "\n" + line;
    }
    private void Awake()
    {
        Debugger.Init();
        Instance = this;
        DontDestroyOnLoad(gameObject);
        m_Mgrs = new List<IManager>();

        m_ObjectMgr = new ObjectManager();
        m_LuaMgr = new LuaManager();
        m_ABMgr = new ABManager();
        m_CutSceneMgr = new CutSceneManager();
        m_ResMgr = new ResourceManager();
        m_UIMgr = new UIManager();
        m_CallMgr = new CallManager();

        m_Mgrs.Add(m_LuaMgr);
        m_Mgrs.Add(m_ABMgr);
        m_Mgrs.Add(m_CutSceneMgr);
        m_Mgrs.Add(m_ResMgr);
        m_Mgrs.Add(m_ObjectMgr);
        m_Mgrs.Add(m_UIMgr);
        m_Mgrs.Add(m_CallMgr);
        
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
        Debugger.OnDestroy();
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