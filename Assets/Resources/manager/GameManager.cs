using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LuaManager m_LuaMgr;
    public ABManager m_ABMgr;
    public CutSceneManager m_CutSceneMgr;
    public ResourceManager m_ResManager;
    public ObjectManager m_ObjectManager;
    private List<IManager> m_Mgrs;
    public Text m_log_object;

    public void Log(string line)
    {
        string text = m_log_object.text;
        m_log_object.text = text + "\n" + line;
    }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        m_Mgrs = new List<IManager>();

        m_LuaMgr = new LuaManager();
        m_ABMgr = new ABManager();
        m_CutSceneMgr = new CutSceneManager();
        m_ResManager = new ResourceManager();
        m_ObjectManager = new ObjectManager();

        m_Mgrs.Add(m_LuaMgr);
        m_Mgrs.Add(m_ABMgr);
        m_Mgrs.Add(m_CutSceneMgr);
        m_Mgrs.Add(m_ResManager);
        m_Mgrs.Add(m_ObjectManager);

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