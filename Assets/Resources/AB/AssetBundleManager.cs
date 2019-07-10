using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace AssetBundles
{
    public class AssetBundleManager : MonoBehaviour
    {
        static string[] m_activeVariants =  {};
        static AssetBundleManifest m_ABManifest = null;

#if UNITY_EDITOR
        static int m_isSimulateInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

        static Dictionary<string, AssetBundleItem> m_loadedABs = new Dictionary<string, AssetBundleItem>();
        static Dictionary<string, string> m_downloadingErrors = new Dictionary<string, string>();
        static List<string> m_downloadingABs = new List<string>();
        static List<ABOperation> m_progressOperations = new List<ABOperation>();
        static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

        public static string[] ActiveVariants
        {
            get { return m_activeVariants; }
            set { m_activeVariants = value; }
        }

        public static AssetBundleManifest AssetBundleManifestObject
        {
            set {m_ABManifest = value; }
        }

        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                return ABManager.CfgLoadMode == LoadModeEnum.EditorOrigin;
            }
            set
            {
                if(value)
                    ABManager.CfgLoadMode = LoadModeEnum.EditorOrigin;
                else
                    ABManager.CfgLoadMode = LoadModeEnum.EditorAB;
            }
        }

        static public AssetBundleItem GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (m_downloadingErrors.TryGetValue(assetBundleName, out error))
                return null;

            AssetBundleItem bundle = null;
            m_loadedABs.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (m_downloadingErrors.TryGetValue(dependency, out error))
                    return null;

                // Wait all the dependent assetBundles being loaded.
                AssetBundleItem dependentBundle;
                m_loadedABs.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }

            return bundle;
        }

        static public bool IsAssetBundleDownloaded(string assetBundleName)
        {
            return m_loadedABs.ContainsKey(assetBundleName);
        }

        static public ABLoadManifestOperation Initialize()
        {
            return Initialize(ABManager.CfgManifestAndPlatformName);
        }

        static public ABLoadManifestOperation Initialize(string manifestAssetBundleName)
        {
#if UNITY_EDITOR
            ABManager.Log(LogType.Info, "Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
#endif

            var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
            DontDestroyOnLoad(go);

#if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't need the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
                return null;
#endif

            LoadAssetBundle(manifestAssetBundleName, true);
            var operation = new ABLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            m_progressOperations.Add(operation);
            return operation;
        }

        static protected void LoadAssetBundle(string assetBundleName)
        {
            LoadAssetBundle(assetBundleName, false);
        }
            
        static public void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            ABManager.Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
                return;
#endif

            if (!isLoadingAssetBundleManifest)
            {
                if (m_ABManifest == null)
                {
                    ABManager.Log(LogType.Error, "Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }

            // Check if the assetBundle has already been processed.
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
                LoadDependencies(assetBundleName);
        }

        static protected string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = m_ABManifest.GetAllAssetBundlesWithVariant();

            // Get base bundle name
            string baseName = assetBundleName.Split('.')[0];

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                int found = System.Array.IndexOf(m_activeVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
                ABManager.Log(LogType.Warning, "Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex]; 
            }
            else
            {
                return assetBundleName;
            }
        }

        static public bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            AssetBundleItem bundle = null;
            m_loadedABs.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.m_referencedCount++;
                return true;
            }

            if (m_downloadingABs.Contains(assetBundleName))
                return true;

            string bundleBaseDownloadingURL = ABManager.CfgServerLoadPath;

            if (bundleBaseDownloadingURL.ToLower().StartsWith("odr://"))
            {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through ODR");
                m_InProgressOperations.Add(new AssetBundleDownloadFromODROperation(assetBundleName));
#else
                new ApplicationException("Can't load bundle " + assetBundleName + " through ODR: this Unity version or build target doesn't support it.");
#endif
            }
            else if (bundleBaseDownloadingURL.ToLower().StartsWith("res://"))
            {
#if ENABLE_IOS_APP_SLICING
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through asset catalog");
                m_InProgressOperations.Add(new AssetBundleOpenFromAssetCatalogOperation(assetBundleName));
#else
                new ApplicationException("Can't load bundle " + assetBundleName + " through asset catalog: this Unity version or build target doesn't support it.");
#endif
            }
            else
            {
                WWW download = null;

                if (!bundleBaseDownloadingURL.EndsWith("/"))
                {
                    bundleBaseDownloadingURL += "/";
                }

                string url = bundleBaseDownloadingURL + assetBundleName;

                // For manifest assetbundle, always download it as we don't have hash for it.
                if (isLoadingAssetBundleManifest)
                    download = new WWW(url);
                else
                    download = WWW.LoadFromCacheOrDownload(url, m_ABManifest.GetAssetBundleHash(assetBundleName), 0);

                m_progressOperations.Add(new ABDownloadWebOperation(assetBundleName, download));
            }
            m_downloadingABs.Add(assetBundleName);

            return false;
        }

        // Where we get all the dependencies and load them all.
        static protected void LoadDependencies(string assetBundleName)
        {
            if (m_ABManifest == null)
            {
                ABManager.Log(LogType.Error, "Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = m_ABManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            m_Dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
                LoadAssetBundleInternal(dependencies[i], false);
        }

        static public void UnloadAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
                return;
#endif
            assetBundleName = RemapVariantName(assetBundleName);

            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }

        static protected void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            m_Dependencies.Remove(assetBundleName);
        }

        static protected void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            AssetBundleItem bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
                return;

            if (--bundle.m_referencedCount == 0)
            {
                bundle.UnLoad();
                m_loadedABs.Remove(assetBundleName);

                ABManager.Log(LogType.Info, assetBundleName + " has been unloaded successfully");
            }
        }

        void Update()
        {
            // Update all in progress operations
            for (int i = 0; i < m_progressOperations.Count;)
            {
                var operation = m_progressOperations[i];
                if (operation.isDependentOtherOperation())
                {
                    i++;
                }
                else
                {
                    m_progressOperations.RemoveAt(i);
                    ProcessFinishedOperation(operation);
                }
            }
        }

        void ProcessFinishedOperation(ABOperation operation)
        {
            ABDownloadOperation download = operation as ABDownloadOperation;
            if (download == null)
                return;

            if (string.IsNullOrEmpty(download.error))
                m_loadedABs.Add(download.m_ABName, download.m_ABItem);
            else
            {
                string msg = string.Format("Failed downloading bundle {0} from {1}: {2}",
                        download.m_ABName, download.GetSourceURL(), download.error);
                m_downloadingErrors.Add(download.m_ABName, msg);
            }

            m_downloadingABs.Remove(download.m_ABName);
        }

        static public ABLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
        {
            ABManager.Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");

            ABLoadAssetOperation operation = null;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                    var bundles = AssetDatabase.GetAllAssetBundleNames();
                    ABManager.Log(LogType.Error, "There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                    return null;
                }

                UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                operation = new ABLoadAssetOperationSimulation(target);
            }
            else
#endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new ABLoadAssetOperationFull(assetBundleName, assetName, type);

                m_progressOperations.Add(operation);
            }

            return operation;
        }

        static public ABOperation LoadSceneAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            ABManager.Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");

            ABOperation operation = null;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                operation = new ABLoadSceneSimulationOperation(assetBundleName, levelName, isAdditive);
            }
            else
#endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new ABLoadSceneOperation(assetBundleName, levelName, isAdditive);

                m_progressOperations.Add(operation);
            }

            return operation;
        }
    }
}
