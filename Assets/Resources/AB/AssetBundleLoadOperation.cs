using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_5_3 || UNITY_5_4
using UnityEngine.SceneManagement;
#endif
#if ENABLE_IOS_ON_DEMAND_RESOURCES
using UnityEngine.iOS;
#endif
using System.Collections;

namespace AssetBundles
{
    // operation操作迭代对象的基类
    public abstract class ABOperation : IEnumerator
    {
        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset() { }

        abstract public bool isDependentOtherOperation();

        abstract public bool IsDone();
    }

    // AB下载操作基类
    public abstract class ABDownloadOperation : ABOperation
    {
        bool m_done;

        public string m_ABName { get; private set; }
        public AssetBundleItem m_ABItem { get; protected set; }
        public string error { get; protected set; }

        protected abstract bool m_isDownloaded { get; }
        protected abstract void FinishDownload();

        public override bool isDependentOtherOperation()
        {
            if (!m_done && m_isDownloaded)
            {
                FinishDownload();
                m_done = true;
            }

            return !m_done;
        }

        public override bool IsDone()
        {
            return m_done;
        }

        public abstract string GetSourceURL();

        public ABDownloadOperation(string ABName)
        {
            this.m_ABName = ABName;
        }
    }

    // 使用HTTP方式异步加载AB包
    public class ABDownloadWebOperation : ABDownloadOperation
    {
        WWW m_WWW;
        string m_url;

        public ABDownloadWebOperation(string ABName, WWW www)
            : base(ABName)
        {
            if (www == null)
                throw new System.ArgumentNullException("www");
            m_url = www.url;
            this.m_WWW = www;
        }

        protected override bool m_isDownloaded { get { return (m_WWW == null) || m_WWW.isDone; } }

        protected override void FinishDownload()
        {
            error = m_WWW.error;
            if (!string.IsNullOrEmpty(error))
                return;

            AssetBundle bundle = m_WWW.assetBundle;
            if (bundle == null)
                error = string.Format("{0} is not a valid asset bundle.", m_ABName);
            else
                m_ABItem = new AssetBundleItem(m_WWW.assetBundle);

            m_WWW.Dispose();
            m_WWW = null;
        }

        public override string GetSourceURL()
        {
            return m_url;
        }
    }

#if UNITY_EDITOR
    // 编辑器模拟模式异步加载AB包中的scene场景
    public class ABLoadSceneSimulationOperation : ABOperation
    {
        AsyncOperation m_asyncOperation = null;

        public ABLoadSceneSimulationOperation(string ABName, string sceneName, bool isAdditive)
        {
            string[] scenePaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(ABName, sceneName);
            if (scenePaths.Length == 0)
            {
                Debug.LogError("There is no scene with name \"" + sceneName + "\" in " + ABName);
                return;
            }
            LoadSceneParameters para = new LoadSceneParameters(LoadSceneMode.Single);
            if (isAdditive)
            {
                para.loadSceneMode = LoadSceneMode.Additive;
            }
            m_asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(scenePaths[0], para);
            m_asyncOperation.allowSceneActivation = true;
        }

        public override bool isDependentOtherOperation()
        {
            return false;
        }

        public override bool IsDone()
        {
            return m_asyncOperation == null || m_asyncOperation.isDone;
        }
    }
#endif
    // AB模式异步加载AB包中的资源
    public class ABLoadSceneOperation : ABOperation
    {
        protected string                m_ABName;
        protected string                m_sceneName;
        protected bool                  m_isAdditive;
        protected string                m_downloadingError;
        protected AsyncOperation m_asyncOperation;

        public ABLoadSceneOperation(string abName, string sceneName, bool isAdditive)
        {
            m_ABName = abName;
            m_sceneName = sceneName;
            m_isAdditive = isAdditive;
        }

        public override bool isDependentOtherOperation()
        {
            if (m_asyncOperation != null)
                return false;

            AssetBundleItem bundleItem = null;
            //GameManager.Instance.m_ABMgr.m_loadedABs.TryGetValue(m_ABName, out bundleItem);
            if (bundleItem != null)
            {
                m_asyncOperation = SceneManager.LoadSceneAsync(m_sceneName, m_isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                return false;
            }
            else
            {
                return true;
            }        
        }

        public override bool IsDone()
        {
            if (m_asyncOperation == null && m_downloadingError != null)
            {
                Debug.LogError(m_downloadingError);
                return true;
            }
            return m_asyncOperation != null && m_asyncOperation.isDone;
        }
    }

    // 异步加载资源抽象基类
    public abstract class ABLoadAssetOperation : ABOperation
    {
        public abstract T GetAsset<T>() where T: UnityEngine.Object;
    }

    public class ABLoadAssetOperationSimulation : ABLoadAssetOperation
    {
        Object   m_simulatedObject;

        public ABLoadAssetOperationSimulation(Object simulatedObject)
        {
            m_simulatedObject = simulatedObject;
        }

        public override T GetAsset<T>()
        {
            return m_simulatedObject as T;
        }

        public override bool isDependentOtherOperation()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }
    }

    // 异步加载AB包和包中Asset资源
    public class ABLoadAssetOperationFull : ABLoadAssetOperation
    {
        protected string m_ABName;
        protected string m_assetName;
        protected string m_downloadingError;
        protected System.Type  m_type;
        protected AssetBundleRequest  m_request = null;

        public ABLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
        {
            m_ABName = bundleName;
            m_assetName = assetName;
            m_type = type;
        }

        public override T GetAsset<T>()
        {
            if (m_request != null && m_request.isDone)
                return m_request.asset as T;
            else
                return null;
        }

        // 如果依赖的bundle还没有被加载则返回True，即还有依赖返回true，没有依赖返回false
        public override bool isDependentOtherOperation()
        {
            //if (m_request != null)
            //    return false;

            //AssetBundleItem bundle = null;
            //           GameManager.Instance.m_ABMgr.m_loadedABs.TryGetValue(m_ABName, out bundle);
            //if (bundle != null)
            //{
            //    ///@TODO: When asset bundle download fails this throws an exception...
            //    m_request = bundle.m_assetBundle.LoadAssetAsync(m_assetName, m_type);
            //    return false;
            //}
            //else
            //{
            return true;
            //}
        }

        public override bool IsDone()
        {
            if (m_request == null && m_downloadingError != null)
            {
                Debug.LogError(m_downloadingError);
                return true;
            }
            return m_request != null && m_request.isDone;
        }
    }

    // 异步加载Mainfest
    public class ABLoadManifestOperation : ABLoadAssetOperationFull
    {
        public ABLoadManifestOperation(string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type)
        {
        }

        public override bool isDependentOtherOperation()
        {
            base.isDependentOtherOperation();

            if (m_request != null && m_request.isDone)
            {
                //AssetBundleManager.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
                return false;
            }
            else
                return true;
        }
    }
}
