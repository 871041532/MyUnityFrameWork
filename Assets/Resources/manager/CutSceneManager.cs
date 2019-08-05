
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager:IManager
{
    public string CurrentSceneName { get; private set; }

    private LoadingSceneLogic m_loadSceneLogic;

    public static int LoadingProgress = 0;

    public override void Awake() {
        m_loadSceneLogic = new LoadingSceneLogic(this);
    }
    public override void Start() {
        //BeginLoadScene("Scene1", () => {
        //    Debug.Log("加载完成！");
        //}, (progress) => {
        //    Debug.Log(string.Format("加载进度: {0}", progress));
        //});
    }

    public void BeginLoadScene(string sceneName, Action successCall, Action<float> progressChangeCall)
    {
        GameMgr.m_CallMgr.TriggerAll(EventEnum.BeginChangeScene, sceneName);
        m_loadSceneLogic.BeginLoading(sceneName, successCall, progressChangeCall);
    }
}

public class LoadingSceneLogic
{
    private AsyncOperation m_async;
    private Action m_successCall;
    private Action<float> m_progressChangeCall;
    private float m_minTime = 1f;
    private float m_startTime = 0;
    private IManager m_owner;

    public LoadingSceneLogic(IManager mgr)
    {
        m_owner = mgr;
    }

    public void SetMinTime(float time)
    {
        m_minTime = time;
    }

    public void BeginLoading(string sceneName, Action successCall, Action<float> progressChangecall)
    {
        m_successCall = successCall;
        m_progressChangeCall = progressChangecall;
        m_owner.GameMgr.StartCoroutine(loadScene(sceneName));
    }

    IEnumerator loadScene(string name)
    {
        m_startTime = Time.time;
        // 异步读取场景
        m_async = SceneManager.LoadSceneAsync(name);
        // 设置加载完成后不自动跳转场景
        m_async.allowSceneActivation = false;
        while (m_async.progress < 0.9f || Time.time - m_startTime < m_minTime*0.9f)
        {
            m_progressChangeCall?.Invoke(Mathf.Min(m_async.progress,(Time.time - m_startTime) / m_minTime));
            yield return null;
        }
        m_async.allowSceneActivation = true;
        while (m_async.progress < 1.0f || Time.time - m_startTime < m_minTime)
        {
            m_progressChangeCall?.Invoke(Mathf.Min(m_async.progress, (Time.time - m_startTime) / m_minTime));
            yield return null;
        }
        // yield return m_async;
        m_owner.GameMgr.m_CallMgr.TriggerAll(EventEnum.OnChangeScene, name);
        m_progressChangeCall?.Invoke(1.0f);
        m_successCall?.Invoke();
    }
}
