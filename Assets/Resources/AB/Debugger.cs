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
    static UdpClient m_UDPClient = null;
    static IPEndPoint m_EndPoint = null;
    static string m_URL = "192.168.1.5";
    static Process m_ChildProcess;
    static LogCallback m_LogCallback;
    public static void Init()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            RunCmd();
        }
        m_LogCallback = ( condition,  stackTrace,  type) => {
            SendUDP(condition);
        };
        Application.logMessageReceived += m_LogCallback;
    }
    static void RunCmd()
    {
        string cmd = string.Format(Environment.CurrentDirectory + "/Tools/DebugTool.py");
        m_ChildProcess = new System.Diagnostics.Process();
        m_ChildProcess.StartInfo.FileName = cmd;
        m_ChildProcess.StartInfo.Arguments = @"";
        m_ChildProcess.Start();
    }

    //    public static void Log(string strs)
    //{
    //    UnityEngine.Debug.Log(strs);
    //    SendUDP(strs);
    //}

    //public static void LogError(string strs)
    //{
    //    UnityEngine.Debug.LogError(strs);
    //    SendUDP(strs);
    //}

    #region UDP客户端
    public static void SendUDP(string sendString)
    {
        if (m_UDPClient is null)
        {
            IPAddress remoteIP = IPAddress.Parse(m_URL); //假设发送给这个IP
            int remotePort = 9000;
            m_EndPoint = new IPEndPoint(remoteIP, remotePort);//实例化一个远程端点 
            m_UDPClient = new UdpClient();
        }
        byte[] sendData = Encoding.UTF8.GetBytes(sendString);
        m_UDPClient.Send(sendData, sendData.Length, m_EndPoint);//将数据发送到远程端点 
    }
    #endregion

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
        Application.logMessageReceived += m_LogCallback;
    }

    #region 标准输出重定义
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AllocConsole();
    private const int STD_OUTPUT_HANDLE = -11;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeConsole();
    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    static extern IntPtr GetStdHandle(int nStdHandle);
    static TextWriter oldOutput;

    internal static void StartWriteline()
    {
        AllocConsole();
        oldOutput = Console.Out;
        try
        {
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            StreamWriter standardOutput = new StreamWriter(fileStream);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            //Console.OutputEncoding = System.Text.Encoding.Unicode;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.Log("Couldn't redirect output: " + ex.Message);
        }
    }

    internal static void CLoseWriteline()
    {
        Console.SetOut(oldOutput);
        FreeConsole();
    }
    #endregion
}