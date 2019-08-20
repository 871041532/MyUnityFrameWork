using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
}

public class CallManager : IManager
{
    private EventManager[] m_EventManagers;

    public CallManager()
    {
        int count = (int)EventType.NoUsingOnlyNum;
        m_EventManagers = new EventManager[count];
        for (int i = 0; i < count; i++)
        {
            m_EventManagers[i] = new EventManager();
        }
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
            foreach (var item in eventRegisters)
            {
                item.Value(args);
            }
        }
    }
}