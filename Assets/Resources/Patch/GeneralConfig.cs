using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GeneralConfig", menuName = "CreateGeneralConfig", order = 0)]
public class GeneralConfig : ScriptableObject
{
    [Header("键值对")]
    public List<StringKStringValue> m_AllFileDirAB = new List<StringKStringValue>();
    
    [System.Serializable]
    public class StringKStringValue
    {
        [Header("键")]
        public string k;
        [Header("值")]
        public string v;
    }
    
    // 运行时动态数据
    public static GeneralConfig StaticCfg;
    private Dictionary<string, string> m_DynamicData = null;

    // 运行时构建
    public static void Init()
    {
        BuildStaticConfig();
    }

    public static void BuildStaticConfig()
    {
        var cfg = Resources.Load<GeneralConfig>("StaticConfig");
        cfg.Build();
        GeneralConfig.StaticCfg = cfg;
    }

    private void Build()
    {
        m_DynamicData = new Dictionary<string, string>();
        foreach (var item in m_AllFileDirAB)
        {
            m_DynamicData[item.k] = item.v;
        }
    }

    // 下标运算符重载
    public string this[string key]
    {
        get
        {
            string v;
            m_DynamicData.TryGetValue(key, out v);
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            else
            {
                return v;
            }
        }
    }
}