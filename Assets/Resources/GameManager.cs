using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LuaMgr m_luaMgr;
    public ABMgr m_abMgr;
    public CutSceneMgr m_cutSceneMgr;
    private List<IMgr> m_mgrs;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        m_mgrs = new List<IMgr>();

        m_luaMgr = new LuaMgr();
        m_abMgr = new ABMgr();
        m_cutSceneMgr = new CutSceneMgr();
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
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "咳咳"))
        {
            m_abMgr.testAb?.Unload(true);
        }
    }
}
