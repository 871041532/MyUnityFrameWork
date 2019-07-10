using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LuaManager m_luaMgr;
    public ABManager m_abMgr;
    public CutSceneManager m_cutSceneMgr;
    private List<IManager> m_mgrs;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        m_mgrs = new List<IManager>();

        m_luaMgr = new LuaManager();
        m_abMgr = new ABManager();
        m_cutSceneMgr = new CutSceneManager();
        m_mgrs.Add(m_luaMgr);
        m_mgrs.Add(m_abMgr);
        m_mgrs.Add(m_cutSceneMgr);

        var iter = m_mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Awake();
        }
    }

    private void Start()
    {
        var iter = m_mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Start();
        }
    }

    private void Update()
    {
        var iter = m_mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Update();
        }
    }

    private void OnDestroy()
    {
        var iter = m_mgrs.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.OnDestroy();
        }
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