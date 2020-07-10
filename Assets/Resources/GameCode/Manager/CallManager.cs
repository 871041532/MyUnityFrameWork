using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum EventType
{
    General,
    NetWork,
    UI,
    All,
    NoUsingOnlyNum,  // 仅仅显示数量用
}

public static class EventEnum
{
    public static string BeginChangeScene = "BeginChangeScene";
    public static string OnChangeScene = "OnChangeScene";
    public static string OnPatched = "OnPatched";
    public static string OnPatchedFail = "OnPatchedFail";
    public static string OnPatchInfoUpdate = "OnPatchInfoUpdate";
    public static string OnUpdate = "OnUpdate";
    public static string OnDestroy = "OnDestroy";
}

public class CallManager : IManager
{
    private EventManager[] m_EventManagers;
    private CallBackManager m_CallbackManager;
    public CallManager()
    {
        int count = (int)EventType.NoUsingOnlyNum;
        m_EventManagers = new EventManager[count];
        for (int i = 0; i < count; i++)
        {
            m_EventManagers[i] = new EventManager();
        }
        m_CallbackManager = new CallBackManager();
    }

    public override void Update()
    {
        m_CallbackManager.Update();
    }

    public void AddInvoke(Action call, float time)
    {
        m_CallbackManager.AddInvoke(call, time);
    }

    public void AddUpdate(Action call)
    {
        m_CallbackManager.AddUpdate(call);
    }

    public void RemoveUpdate(Action call)
    {
        m_CallbackManager.RemoveUpdate(call);
    }
    
    public void RegisterEvent(string eventName, Action<object[]> call, EventType type = EventType.General)
    {
        m_EventManagers[(int)type].RegisterEvent(eventName, call);
    }

    public void RemoveEvent(string eventName, Action<object[]> call, EventType type = EventType.General)
    {
        m_EventManagers[(int)type].RemoveEvent(eventName, call);
    }

    public void TriggerEvent(string eventName, EventType type = EventType.General, params object[] args)
    {
        m_EventManagers[(int)type].TriggerEvent(eventName, args);
    }

    public void TriggerAll(string eventName, params object[] args)
    {
        for (int i = 0; i < m_EventManagers.Length; i++)
        {
            m_EventManagers[i].TriggerEvent(eventName, args);
        }
    }

    public override void OnDestroy() {
        m_CallbackManager.Clear();
        foreach (var item in m_EventManagers)
        {
            item.Clear();
        }
    }
}

public class CallBackManager
{
    public SortedDictionary<int, List<Action>> m_TimeRegisters = new SortedDictionary<int, List<Action>>();
    public Dictionary<int, Action> m_UpdateRegisters = new Dictionary<int, Action>();

    public void AddInvoke(Action call, float time)
    {
        int targetFrame = Mathf.FloorToInt((Time.time + time) * 30);
        List<Action> list;
        m_TimeRegisters.TryGetValue(targetFrame, out list);
        if (list is null)
        {
            int canUseKey = -1;
            var iter = m_TimeRegisters.GetEnumerator();
            if (iter.MoveNext())
            {
                if (iter.Current.Value.Count == 0)
                {
                    canUseKey = iter.Current.Key;
                }
            }
            if (canUseKey != -1)
            {
                list = m_TimeRegisters[canUseKey];
                m_TimeRegisters.Remove(canUseKey);
            }
            else
            {
                list = new List<Action>();
            }
            m_TimeRegisters.Add(targetFrame, list);
        }
        list.Add(call);
    }
    public void AddUpdate(Action call)
    {
        int hashCode = call.GetHashCode();
        m_UpdateRegisters.Add(hashCode, call);
    }


    public void RemoveUpdate(Action call)
    {
        m_UpdateRegisters.Remove(call.GetHashCode());
    }

    List<int> m_tempCallList = new List<int>();
    public void Update()
    {
        foreach (var item in m_UpdateRegisters)
        {
            item.Value();
        }

        int targetFrame = Mathf.FloorToInt((Time.time) * 30);
        m_tempCallList.AddRange(m_TimeRegisters.Keys);
        foreach (var key in m_tempCallList)
        {
            var list = m_TimeRegisters[key];
            var isCall = false;
            if (key <= targetFrame)
            {
                foreach (var call in list)
                {
                    call();
                }
                isCall = true;
            }
            if (isCall)
            {
                list.Clear();
            }
        }
        m_tempCallList.Clear();
    }

    public void Clear()
    {
        m_TimeRegisters.Clear();
        m_UpdateRegisters.Clear();
    }
}

public class EventManager
{
    public Dictionary<string, Dictionary<int, Action<object[]>>> m_Registers = new Dictionary<string, Dictionary<int, Action<object[]>>>();
    public void RegisterEvent(string eventName, Action<object[]> call)
    {
        Dictionary <int, Action<object[]>> eventRegisters = null;
        m_Registers.TryGetValue(eventName, out eventRegisters);
        if (eventRegisters is null)
        {
            eventRegisters = new Dictionary<int, Action<object[]>>();
            m_Registers.Add(eventName, eventRegisters);
        }

        int hashCode = call.GetHashCode();
        if (!eventRegisters.ContainsKey(hashCode))
        {
            eventRegisters.Add(hashCode, call);
        }
    }

    public void RemoveEvent(string eventName, Action<object[]> call)
    {
        Dictionary<int, Action<object[]>> eventRegisters = null;
        m_Registers.TryGetValue(eventName, out eventRegisters);
        if (!(eventRegisters is null))
        {
            int hashCode = call.GetHashCode();
            if (eventRegisters.ContainsKey(hashCode))
            {
                eventRegisters.Remove(hashCode);
            }
        }
    }

    public void TriggerEvent(string eventName, params object[] args)
    {
        Dictionary<int, Action<object[]>> eventRegisters = null;
        m_Registers.TryGetValue(eventName, out eventRegisters);
        if (!(eventRegisters is null))
        {
            var keys = new List<int>(eventRegisters.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                eventRegisters[keys[i]](args);
            }
        }
    }

    public void Clear()
    {
        m_Registers.Clear();
    }
}