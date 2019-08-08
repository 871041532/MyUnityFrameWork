using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using static UnityEngine.Application;

public static class Debugger
{
    static bool is_OpenDebug = true;
    static string m_UDPServerURL = "127.0.0.1";
    static string m_UDPServerPort = "10000";

    static UdpClient m_UDPClient = null;
    static IPEndPoint m_EndPoint = null;
    static Process m_ChildProcess;
    static LogCallback m_LogCallback;
    static UnityEngine.LogType m_LastLogType;

    public static void Init()
    {
        if (!is_OpenDebug)
        {
            return;
        }
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            RunPythonConsole();
        }
        m_LogCallback = ( condition,  stackTrace,  type) => {
            if (m_LastLogType != type)
            {
                m_LastLogType = type;
                SendUDP("#" + (int)type);
            }
            SendUDP(condition);
            if (type != UnityEngine.LogType.Log)
            {
                SendUDP(stackTrace);
            }         
        };
        Application.logMessageReceived += m_LogCallback;
    }

    static void RunPythonConsole()
    {
        string cmd = string.Format(Environment.CurrentDirectory + "/Tools/DebugTool.py");
        m_ChildProcess = new System.Diagnostics.Process();
        m_ChildProcess.StartInfo.FileName = cmd;
        m_ChildProcess.StartInfo.Arguments = m_UDPServerPort;
        m_ChildProcess.Start();
    }

    public static void SendUDP(string sendString)
    {
        if (m_UDPClient is null)
        {
            IPAddress remoteIP = IPAddress.Parse(m_UDPServerURL); //假设发送给这个IP
            m_EndPoint = new IPEndPoint(remoteIP, Convert.ToInt32(m_UDPServerPort));//实例化一个远程端点 
            m_UDPClient = new UdpClient();
        }
        byte[] sendData = Encoding.UTF8.GetBytes(sendString);
        m_UDPClient.Send(sendData, sendData.Length, m_EndPoint);//将数据发送到远程端点 
    }

    public static void OnDestroy()
    {
        if (!(m_UDPClient is null))
        {
            m_UDPClient.Close();
            m_UDPClient = null;
        }
        if (!(m_ChildProcess is null))
        {
            try
            {
                m_ChildProcess.Kill();
            }
            catch (Exception)
            {
            }
            m_ChildProcess = null;
        }
        if (is_OpenDebug)
        {
            Application.logMessageReceived -= m_LogCallback;
        }  
    }
}